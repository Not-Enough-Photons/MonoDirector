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
        }

        public static Playback instance;

        private Coroutine playRoutine;

        public void BeginPlayback()
        {
            if (playRoutine == null)
            {
                playRoutine = MelonCoroutines.Start(PlayRoutine()) as Coroutine;
            }
        }

        public void OnPrePlayback()
        {
            //Director.instance.PlayState = PlayState.Playing;
            //currentTick = 0;

            foreach (var castMember in Director.instance.Cast)
            {
                castMember.Act(0);
                castMember.ShowActor(true);
            }

            foreach (var prop in Director.instance.WorldProps)
            {
                if (prop == null)
                {
                    continue;
                }
                else
                {
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

            /*if (currentTick < RecordedTicks - 1)
            {
                currentTick++;
            }*/

            foreach (var castMember in Director.instance.Cast)
            {
                AnimateActor(Director.instance.CurrentTick, castMember);
            }

            foreach (var prop in Director.instance.WorldProps)
            {
                AnimateProp(Director.instance.CurrentTick, prop);
            }
        }

        public void OnStopPlayback()
        {
            if (playRoutine != null)
            {
                MelonCoroutines.Stop(playRoutine);
                playRoutine = null;
            }
        }

        public void AnimateActor(int frame, Actor actor)
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
