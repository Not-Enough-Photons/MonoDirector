using System;
using System.Collections;
using System.Threading;
using MelonLoader;

using NEP.MonoDirector.Actors;
using NEP.MonoDirector.State;
using UnhollowerBaseLib;
using UnityEngine;

namespace NEP.MonoDirector.Core
{
    public class Recorder
    {
        public Recorder()
        {
            instance = this;

            Events.OnPreRecord += OnPreRecord;
            Events.OnRecordTick += OnRecordTick;
            Events.OnStopRecording += OnStopRecording;
        }

        public static Recorder instance;

        public int RecordTick { get => recordTick; }

        public float TimeRecording { get => timeRecording; }

        public ActorPlayer ActiveActor { get => activeActor; }

        private ActorPlayer activeActor;

        private Coroutine recordRoutine;

        private int recordTick;

        private float timeRecording;

        public void BeginRecording()
        {
            if (recordRoutine == null)
            {
                recordRoutine = MelonCoroutines.Start(RecordRoutine()) as Coroutine;
            }
        }

        public void RecordCamera()
        {
            /*foreach (var castMember in Director.instance.Cast)
            {
                castMember?.Act(Director.instance.WorldTick);
            }*/
        }

        public void RecordActor()
        {
            activeActor.Capture();

            foreach (var prop in Director.instance.RecordingProps)
            {
                prop.Record(recordTick);
            }

            foreach (var castMember in Director.instance.Cast)
            {
                Playback.instance.AnimateActor(recordTick, castMember);
            }

            foreach(var npcCastMember in Director.instance.NPCCast)
            {
                Playback.instance.AnimateNPC(recordTick, npcCastMember);
            }

            foreach(var prop in Director.instance.WorldProps)
            {
                Playback.instance.AnimateProp(recordTick, prop);
            }
        }

        public void OnPreRecord()
        {
            if (recordTick > 0)
            {
                recordTick = 0;
            }

            timeRecording = 0f;

            activeActor = new ActorPlayer(Constants.rigManager.avatar);

            foreach (var castMember in Director.instance.Cast)
            {
                if (castMember != null)
                {

                }
            }
        }

        public void OnRecordTick()
        {
            if (Director.PlayState == PlayState.Paused)
            {
                return;
            }

            recordTick++;

            if (Director.CaptureState == CaptureState.CaptureCamera)
            {
                RecordCamera();
            }

            if (Director.CaptureState == CaptureState.CaptureActor)
            {
                RecordActor();
            }

            timeRecording += Time.deltaTime;
        }

        public void OnStopRecording()
        {
            Director.instance.Cast.Add(activeActor);

            activeActor = null;

            Director.instance.WorldProps.AddRange(Director.instance.RecordingProps);
            Director.instance.RecordingProps.Clear();

            if (recordRoutine != null)
            {
                MelonCoroutines.Stop(recordRoutine);
                recordRoutine = null;
            }
        }

        public IEnumerator RecordRoutine()
        {
            Events.OnPreRecord?.Invoke();
            yield return new WaitForSeconds(5f);
            Events.OnStartRecording?.Invoke();

            while(Director.PlayState == PlayState.Recording)
            {
                Events.OnRecordTick?.Invoke();
                yield return new WaitForEndOfFrame();
            }

            Events.OnStopRecording?.Invoke();

            yield return null;
        }
    }
}
