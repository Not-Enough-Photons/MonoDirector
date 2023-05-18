using System.Collections;

using NEP.MonoDirector.Actors;

using SLZ.VRMK;

using UnityEngine;

using Avatar = SLZ.VRMK.Avatar;

namespace NEP.MonoDirector
{
    public class Photographer
    {
        public Photographer()
        {
            Events.OnPrePhotograph += OnPrePhotograph;
            Events.OnPhotograph += OnPhotograph;
            Events.OnPostPhotograph += OnPostPhotograph;
        }

        public int Delay { get; set; }

        private Actor activeActor;
        private Actor lastActor;

        public void SetActor(Avatar avatar)
        {
            lastActor = activeActor;
            activeActor = new Actor(avatar);
        }

        public void OnPrePhotograph()
        {

        }

        public void OnPhotograph()
        {

        }

        public void OnPostPhotograph()
        {

        }

        public IEnumerator PhotographRoutine()
        {
            Events.OnPrePhotograph?.Invoke();

            for(int i = 0; i < Delay; i++)
            {
                Events.OnTimerCountdown?.Invoke();
                yield return new WaitForSeconds(Delay);
            }

            Events.OnPhotograph?.Invoke();

            Events.OnPostPhotograph?.Invoke();

            yield return null;
        }
    }
}
