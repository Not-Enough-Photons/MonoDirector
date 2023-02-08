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

        public void OnPrePlayback()
        {
            Director.instance.PlayState = PlayState.Playing;
            currentTick = 0;

            foreach (var castMember in Cast)
            {
                castMember.Act(0);
                castMember.ShowActor(true);
            }

            foreach (var prop in WorldProps)
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
            if (playState == PlayState.Paused)
            {
                return;
            }

            if (currentTick < RecordedTicks - 1)
            {
                currentTick++;
            }

            foreach (var castMember in Cast)
            {
                castMember.Act(currentTick);
            }

            foreach (var prop in WorldProps)
            {
                prop.Play(currentTick);
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

            while (playState == PlayState.Playing || playState == PlayState.Paused)
            {
                Events.OnPlaybackTick?.Invoke();
                yield return null;
            }

            Events.OnStopPlayback?.Invoke();
            yield return null;
        }
    }
}
