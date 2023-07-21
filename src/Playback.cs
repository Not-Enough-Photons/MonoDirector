using System;
using System.Collections;
using Il2CppSystem;
using MelonLoader;
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.State;

using UnityEngine;

namespace NEP.MonoDirector.Core
{
    public class Playback
    {
        public Playback()
        {
            instance = this;

            Events.OnPrePlayback += OnPrePlayback;
            Events.OnPlay += OnPlay;
            Events.OnPlaybackTick += OnPlaybackTick;
            Events.OnStopPlayback += OnStopPlayback;
        }

        public static Playback instance;

        public int PlaybackTick { get => playbackTick; }

        public float PlaybackTime { get => playbackTime; }

        public float PlaybackRate { get => playbackRate; }

        public int Countdown { get; private set; }

        private Coroutine playRoutine;

        private int playbackTick;

        private float playbackTime;

        private float playbackRate = 1f;

        private float playbackFPSTimer;

        public void SetPlaybackRate(float rate)
        {
            this.playbackRate = rate;
        }

        public void Tick()
        {
            if (Director.PlayState != PlayState.Playing)
            {
                return;
            }

            Events.OnPlaybackTick?.Invoke();
        }

        public void BeginPlayback()
        {
            if(Director.LastPlayState == PlayState.Paused)
            {
                Director.instance.SetPlayState(PlayState.Playing);
                return;
            }

            if (playRoutine == null)
            {
                playRoutine = MelonCoroutines.Start(PlayRoutine()) as Coroutine;
            }
        }

        public void OnPrePlayback()
        {
            playbackFPSTimer = 0f;

            playbackTick = 0;

            ResetPlayhead();

            foreach (var castMember in Director.instance.Cast)
            {
                castMember.OnSceneBegin();
            }

            foreach (var prop in Director.instance.WorldProps)
            {
                prop.OnSceneBegin();
                prop.gameObject.SetActive(true);
            }
        }

        public void OnPlay()
        {
            foreach(var actor in Director.instance.Cast)
            {
                if(actor is Actor actorPlayer)
                {
                    actorPlayer?.Microphone?.Playback();
                }
            }
        }

        public void OnPlaybackTick()
        {
            if (Director.PlayState == PlayState.Stopped || Director.PlayState == PlayState.Paused)
            {
                return;
            }

            AnimateAll();
            
            playbackTime += playbackRate * Time.deltaTime;
        }

        public void OnStopPlayback()
        {
            foreach (Trackable castMember in Director.instance.Cast)
            {
                if (castMember != null && castMember is Actor actorPlayer)
                {
                    actorPlayer?.Microphone?.StopPlayback();
                }
            }

            if (playRoutine != null)
            {
                MelonCoroutines.Stop(playRoutine);
                playRoutine = null;
            }
        }

        public void Seek(float rate)
        {
            if(Director.PlayState != PlayState.Stopped)
            {
                return;
            }

            if(playbackTime <= 0f)
            {
                playbackTime = 0f;
            }

            if(playbackTime >= Recorder.instance.TakeTime)
            {
                playbackTime = Recorder.instance.TakeTime;
            }

            AnimateAll();

            playbackTime += rate * Time.deltaTime;
        }

        public void ResetPlayhead()
        {
            playbackTime = 0f;
        }

        public void MovePlayhead(float amount)
        {
            playbackTime += amount;
        }

        public void AnimateAll()
        {
            foreach (var castMember in Director.instance.Cast)
            {
                AnimateActor(castMember);
            }

            foreach (var prop in Director.instance.WorldProps)
            {
                AnimateProp(prop);
            }
        }

        public void AnimateActor(Trackable actor)
        {
            if(actor == null)
            {
                return;
            }

            actor.Act();
        }

        public void AnimateProp(Prop prop)
        {
            if(prop == null)
            {
                return;
            }

            prop.Act();
        }

        public IEnumerator PlayRoutine()
        {
            Events.OnPrePlayback?.Invoke();

           for (Countdown = 0; Countdown < Settings.World.delay; Countdown++)
            {
                Events.OnTimerCountdown?.Invoke();
                yield return new WaitForSeconds(1);
            }

            Main.feedbackSFX.BeepHigh();

            Events.OnPlay?.Invoke();

            while (Director.PlayState == PlayState.Playing || Director.PlayState == PlayState.Paused)
            {
                while (Director.PlayState == PlayState.Paused)
                {
                    yield return null;
                }

                if (PlaybackTime >= Recorder.instance.TakeTime)
                {
                    break;
                }

                Tick();
                
                yield return null;
            }

            Events.OnStopPlayback?.Invoke();
            yield return null;
        }
    }
}
