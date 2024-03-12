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
            Instance = this;

            Events.OnPrePlayback += OnPrePlayback;
            Events.OnPlay += OnPlay;
            Events.OnPlaybackTick += OnPlaybackTick;
            Events.OnStopPlayback += OnStopPlayback;
        }

        public static Playback Instance;


        private float playbackTime;
        
        /// <summary>
        /// The current time stamp of the playhead
        /// </summary>
        public float PlaybackTime => playbackTime;
        
        /// <summary>
        /// The rate at which the playhead seeks, similar to Time.timeScale
        /// </summary>
        public float PlaybackRate = 1;
        // TODO: Should this be private for some reason?

        public int Countdown { get; private set; }

        private Coroutine playRoutine;

        //
        // Playback modification methods
        //
        public void ResetPlayhead() => playbackTime = 0f;

        public void MovePlayhead(float amount) => playbackTime += amount;

        //
        // Playback methods
        //
        
        /// <summary>
        /// Called per frame, invokes any and all OnPlaybackTick delegates
        /// </summary>
        public void Tick()
        {
            if (Director.PlayState != PlayState.Playing)
                return;

            Events.OnPlaybackTick?.Invoke();
        }
        
        /// <summary>
        /// Called when playback is requested to start
        /// This spawns a coroutine that waits until a delay has passed to begin playing
        /// </summary>
        public void BeginPlayback()
        {
            if(Film.Instance == null)
            {
                return;
            }

            if(!Film.Instance.HasActiveScene() || !Film.Instance.HasScenes())
            {
                return;
            }

            if (Director.LastPlayState == PlayState.Paused)
            {
                Director.instance.SetPlayState(PlayState.Playing);
                return;
            }

            if (playRoutine == null)
                playRoutine = MelonCoroutines.Start(PlayRoutine()) as Coroutine;
        }

        /// <summary>
        /// Called before playback begins
        /// This resets the scene state and playhead
        /// </summary>
        public void OnPrePlayback()
        {
            ResetPlayhead();

            foreach (var castMember in Film.Instance.ActiveScene.Actors)
            {
                castMember.OnSceneBegin();
            }

            foreach (var prop in Film.Instance.ActiveScene.Props)
            {
                prop.OnSceneBegin();
                prop.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Called during playback
        /// </summary>
        public void OnPlay()
        {
            foreach(var actor in Film.Instance.ActiveScene.Actors)
            {
                if(actor is Actor actorPlayer)
                {
                    actorPlayer?.Microphone?.Playback();
                }
            }
        }

        /// <summary>
        /// Called per playback tick
        /// </summary>
        public void OnPlaybackTick()
        {
            if (Director.PlayState == PlayState.Stopped || Director.PlayState == PlayState.Paused)
            {
                return;
            }

            AnimateAll();
            
            playbackTime += PlaybackRate * Time.deltaTime;
        }

        /// <summary>
        /// Called when playback is requested to stop
        /// </summary>
        public void OnStopPlayback()
        {
            foreach (Trackable castMember in Film.Instance.ActiveScene.Actors)
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

        /// <summary>
        /// Manually seeks the playback head in the provided direction
        /// Negative seconds will reverse the playback 
        /// </summary>
        /// <param name="amount">The amount of seconds to seek the playback</param>
        public void Seek(float amount)
        {
            if (Director.PlayState != PlayState.Stopped)
                return;

            if (playbackTime <= 0f)
                playbackTime = 0f;

            if (playbackTime >= Recorder.instance.TakeTime)
                playbackTime = Recorder.instance.TakeTime;

            AnimateAll();

            playbackTime += amount;
        }

        /// <summary>
        /// Animates all tracked scene objects
        /// Call when playback head is seeked to make sure changes are applied!
        /// </summary>
        public void AnimateAll()
        {
            foreach (var castMember in Film.Instance.ActiveScene.Actors)
                AnimateActor(castMember);

            foreach (var prop in Film.Instance.ActiveScene.Props)
                AnimateProp(prop);
        }
        
        /// <summary>
        /// Animates the provided actor
        /// </summary>
        /// <param name="actor">The actor to "act"</param>
        public void AnimateActor(Trackable actor)
        {
            if (actor != null)
                actor.Act();
        }

        /// <summary>
        /// Animates the provided prop
        /// </summary>
        /// <param name="prop">The prop to "act"</param>
        public void AnimateProp(Prop prop)
        {
            if (prop != null)
                prop.Act();
        }

        /// TODO: Is PlayRoutine() having a delay necessary?
        
        /// <summary>
        /// A routine that delays playback by a certain delay
        /// </summary>
        /// <returns></returns>
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
                // TODO: Replace this with WaitUntil to prevent Coroutine garbage?
                while (Director.PlayState == PlayState.Paused)
                    yield return null;

                if (PlaybackTime >= Recorder.instance.TakeTime)
                    break;

                Tick();
                
                yield return null;
            }

            Events.OnStopPlayback?.Invoke();
            yield return null;
        }
    }
}
