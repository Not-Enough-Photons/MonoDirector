using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.State;

using UnityEngine;

using Avatar = SLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Core
{
    public class Recorder
    {
        public Recorder()
        {
            instance = this;

            Events.OnPreRecord += OnPreRecord;
            Events.OnStartRecording += OnPostRecord;
            Events.OnRecordTick += OnRecordTick;
            Events.OnStopRecording += OnStopRecording;
        }

        public static Recorder instance;

        public float RecordingTime { get => recordingTime; }

        public int RecordTick { get => recordTick; }

        public List<Actor> ActiveActors = new List<Actor>();

        public Actor ActiveActor { get => activeActor; }
        public Actor LastActor { get => lastActor; }

        private Actor activeActor;
        private Actor lastActor;

        private Coroutine recordRoutine;

        private float recordingTime;

        private int recordTick;

        public void SetActor(Avatar avatar)
        {
            lastActor = activeActor;
            activeActor = new Actor(avatar);
        }

        public void Tick()
        {
            if (Director.PlayState != PlayState.Recording)
            {
                return;
            }

            Events.OnRecordTick?.Invoke();
        }

        public void StartRecordRoutine()
        {
            if (recordRoutine == null)
            {
                recordRoutine = MelonCoroutines.Start(RecordRoutine()) as Coroutine;
            }
        }

        public void RecordCamera()
        {
            foreach (var castMember in Director.instance.Cast)
            {
                castMember?.Act();
            }
        }

        public void RecordActor()
        {
            activeActor.RecordFrame();

            foreach (var prop in Director.instance.RecordingProps)
            {
                prop.Record(recordTick);
            }

            foreach (var castMember in Director.instance.Cast)
            {
                Playback.instance.AnimateActor(castMember);
            }

            foreach(var prop in Director.instance.WorldProps)
            {
                Playback.instance.AnimateProp(prop);
            }
        }

        /// <summary>
        /// Called when we first hit the record button.
        /// </summary>
        public void OnPreRecord()
        {
            if (recordTick > 0)
            {
                recordTick = 0;
            }

            Playback.instance.ResetPlayhead();

            recordingTime = 0f;

            SetActor(Constants.rigManager.avatar);

            foreach (var castMember in Director.instance.Cast)
            {
                castMember.OnSceneBegin();
            }

            foreach(var prop in Director.instance.WorldProps)
            {
                prop.OnSceneBegin();
                prop.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Called the moment the recording begins.
        /// </summary>
        public void OnPostRecord()
        {
            activeActor?.Microphone?.RecordMicrophone();

            foreach (Trackable castMember in Director.instance.Cast)
            {
                if (castMember != null && castMember is Actor actorPlayer)
                {
                    actorPlayer?.Microphone?.Playback();
                }
            }
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public void OnRecordTick()
        {
            if (Director.PlayState == PlayState.Paused)
            {
                return;
            }

            recordTick++;
            recordingTime += Time.deltaTime;

            Playback.instance.MovePlayhead(Time.deltaTime);

            if (Director.CaptureState == CaptureState.CaptureCamera)
            {
                RecordCamera();
            }
            
            if (Director.CaptureState == CaptureState.CaptureActor)
            {
                RecordActor();
            }

            foreach (var castMember in Director.instance.Cast)
            {
                if (castMember != null)
                {
                    castMember.Act();
                }
            }
        }

        /// <summary>
        /// Called when the recording stops.
        /// </summary>
        public void OnStopRecording()
        {
            activeActor?.Microphone?.StopRecording();

            foreach (Trackable castMember in Director.instance.Cast)
            {
                if (castMember != null && castMember is Actor actorPlayer)
                {
                    actorPlayer?.Microphone?.StopPlayback();
                }
            }

            activeActor.CloneAvatar();
            Director.instance.Cast.Add(activeActor);
            lastActor = activeActor;

            activeActor = null;

            Director.instance.Cast.AddRange(ActiveActors);
            ActiveActors.Clear();

            Director.instance.WorldProps.AddRange(Director.instance.RecordingProps);
            Director.instance.LastRecordedProps = Director.instance.RecordingProps;
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

            for (int i = 0; i < 3; i++)
            {
                Main.feedbackSFX.BeepLow();
                yield return new WaitForSeconds(1);
            }

            Main.feedbackSFX.BeepHigh();

            Events.OnStartRecording?.Invoke();

            while (Director.PlayState == PlayState.Recording || Director.PlayState == PlayState.Paused)
            {
                Tick();
                yield return null;
            }
            
            Events.OnStopRecording?.Invoke();
            yield return null;
        }
    }
}
