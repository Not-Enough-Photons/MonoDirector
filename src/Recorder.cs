using System.Collections;
S
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.State;

using UnityEngine;

namespace NEP.MonoDirector.Core
{
    public class Recorder
    {
        public Recorder()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public static Recorder instance;

        private Actor currentRecordingActor;

        private IEnumerator recordRoutine;

        public void RecordCamera()
        {
            foreach (var castMember in Director.instance.Cast)
            {
                castMember?.Act(Director.instance.WorldTick);
            }
        }

        public void RecordActor()
        {
            foreach (var castMember in Director.instance.Cast)
            {
                castMember?.Act(Director.instance.WorldTick);
            }

            currentRecordingActor.CaptureAvatarFrame();

            foreach (var prop in Director.instance.WorldProps)
            {
                prop.Record(Director.instance.WorldTick);
            }
        }

        public void OnPreRecord()
        {
            // playState = PlayState.Recording;

            /*if (Director.instance.RecordedTicks > 0)
            {
                worldTick = 0;
            }*/

            currentRecordingActor = new Actor(Constants.rigManager.avatar);

            foreach (var castMember in Director.instance.Cast)
            {
                if (castMember != null)
                {
                    castMember.Act(0);
                    castMember.ShowActor(true);
                }
            }
        }

        public void OnRecordTick()
        {
            if (Director.PlayState == PlayState.Paused)
            {
                return;
            }

            //worldTick++;

            if (Director.CaptureState == CaptureState.CaptureCamera)
            {
                RecordCamera();
            }

            if (Director.CaptureState == CaptureState.CaptureActor)
            {
                RecordActor();
            }
        }

        public void OnStopRecording()
        {
            if (Director.instance.RecordedTicks <= Director.instance.WorldTick && Director.instance.RecordedTicks != Director.instance.WorldTick)
            {
                // TODO: refactor so the director updates this properly
                //recordedTicks = worldTick;
            }

            currentRecordingActor.CloneAvatar();
            Director.instance.Cast.Add(currentRecordingActor);

            currentRecordingActor = null;

            if (recordRoutine != null)
            {
                MelonLoader.MelonCoroutines.Stop(recordRoutine);
                recordRoutine = null;
            }
        }

        public IEnumerator RecordRoutine()
        {
            Events.OnPreRecord?.Invoke();
            yield return new WaitForSeconds(5f);
            Events.OnStartRecording?.Invoke();

            while (Director.PlayState == PlayState.Recording || Director.PlayState == PlayState.Paused)
            {
                Events.OnRecordTick?.Invoke();
                yield return null;
            }

            Events.OnStopRecording?.Invoke();
            yield return null;
        }
    }
}
