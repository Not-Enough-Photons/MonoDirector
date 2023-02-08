using System.Collections;

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
                castMember.Act(Director.instance.CurrentTick);
            }

            foreach (var prop in Director.instance.WorldProps)
            {
                prop.Play(Director.instance.CurrentTick);
            }
        }

        public void OnStopPlayback()
        {
            if (playRoutine != null)
            {
                MelonLoader.MelonCoroutines.Stop(playRoutine);
                playRoutine = null;
            }
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
