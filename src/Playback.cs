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
            if(instance == null)
            {
                instance = this;
            }

            Events.OnPrePlayback += OnPrePlayback;
            Events.OnPlaybackTick += OnPlaybackTick;
            Events.OnStopPlayback += OnStopPlayback;
        }

        public static Playback instance;

        public int PlaybackTick { get => playbackTick; }

        private Coroutine playRoutine;

        private int playbackTick;

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

            foreach (var castMember in Director.instance.Cast)
            {
                AnimateActor(0, castMember);
            }

            foreach (var prop in Director.instance.WorldProps)
            {
                if (prop == null)
                {
                    continue;
                }
                else
                {
                    AnimateProp(0, prop);
                    prop.gameObject.SetActive(true);
                }
            }
        }

        public void OnPlaybackTick()
        {
            if (Director.PlayState == PlayState.Paused)
            {
                return;
            }

            AnimateAll(playbackTick);

            playbackTick++;
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

            AnimateAll(playbackTick);
            playbackTick += rate;
        }

        public void AnimateAll(int frame)
        {
            foreach (var castMember in Director.instance.Cast)
            {
                AnimateActor(frame, castMember);
            }

            foreach (var prop in Director.instance.WorldProps)
            {
                AnimateProp(frame, prop);
            }
        }

        public void AnimateActor(int frame, Actor actor)
        {
            if(actor == null)
            {
                return;
            }

            if(actor is ActorPlayer playerActor)
            {
                playerActor.Act(frame);
            }

            actor.Act(frame);
        }

        public void AnimateNPC(int frame, ActorNPC actor)
        {
            if(actor == null)
            {
                return;
            }

            actor.Act(frame);
        }

        public void AnimateProp(int frame, ActorProp prop)
        {
            if(prop == null)
            {
                return;
            }

            prop.Play(frame);
        }

        public IEnumerator PlayRoutine()
        {
            Events.OnPrePlayback?.Invoke();
            yield return new WaitForSeconds(4f);
            Events.OnPlay?.Invoke();

            while (Director.PlayState == PlayState.Playing || Director.PlayState == PlayState.Paused)
            {
                Events.OnPlaybackTick?.Invoke();
                yield return null;
            }

            Events.OnStopPlayback?.Invoke();
            yield return null;
        }
    }
}
