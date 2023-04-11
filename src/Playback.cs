using System;
using System.Collections;
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
            Events.OnPlaybackTick += OnPlaybackTick;
            Events.OnStopPlayback += OnStopPlayback;
        }

        public static Playback instance;

        public int PlaybackTick { get => playbackTick; }

        public float PlaybackTime { get => playbackTime; }

        private Coroutine playRoutine;

        private int playbackTick;

        private float playbackTime;

        public void LateUpdate()
        {
            if (Director.PlayState != PlayState.Playing)
            {
                return;
            }

            Events.OnPlaybackTick?.Invoke();
        }

        public void BeginPlayback()
        {
            if (playRoutine == null)
            {
                playRoutine = MelonCoroutines.Start(PlayRoutine()) as Coroutine;
            }
        }

        public void OnPrePlayback()
        {
            playbackTick = 0;
            playbackTime = 0f;

            foreach (var castMember in Director.instance.Cast)
            {
                AnimateActor(castMember);
            }

            foreach (var prop in Director.instance.WorldProps)
            {
                if (prop == null)
                {
                    continue;
                }
                else
                {
                    AnimateProp(prop);
                    prop.gameObject.SetActive(true);
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

            playbackTick++;
            playbackTime += Time.deltaTime;
        }

        public void OnStopPlayback()
        {
            if (playRoutine != null)
            {
                MelonCoroutines.Stop(playRoutine);
                playRoutine = null;
            }
        }

        public void Seek(int rate)
        {
            if(Director.PlayState != PlayState.Stopped)
            {
                return;
            }

            if(playbackTick <= 0)
            {
                playbackTick = 0;
            }

            if(playbackTick > Recorder.instance.RecordTick)
            {
                playbackTick = Recorder.instance.RecordTick;
            }

            AnimateAll();
            playbackTick += rate;
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

        public void AnimateActor(Actor actor)
        {
            if(actor == null)
            {
                return;
            }

            actor.Act();
        }

        public void AnimateProp( ActorProp prop)
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
            yield return new WaitForSeconds(4f);
            Events.OnPlay?.Invoke();

            while (Director.PlayState == PlayState.Playing || Director.PlayState == PlayState.Paused)
            {
                LateUpdate();
                yield return null;
            }

            Events.OnStopPlayback?.Invoke();
            yield return null;
        }
    }
}
