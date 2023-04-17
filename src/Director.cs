﻿using UnityEngine;

using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.State;

using System.Collections.Generic;
using NEP.MonoDirector.Actors;
using UnityEngine.Splines;
using BoneLib;

namespace NEP.MonoDirector.Core
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class Director : MonoBehaviour
    {
        public Director(System.IntPtr ptr) : base(ptr) { }

        public static Director instance { get; private set; }

        public Playback playback;
        public Recorder recorder;

        public FreeCameraRig Camera { get => camera; }

        public static PlayState PlayState { get => playState; }
        public static CaptureState CaptureState { get => captureState; }

        public List<ActorPlayer> Cast;
        public List<ActorNPC> NPCCast;
        public List<ActorProp> WorldProps;
        public List<ActorProp> RecordingProps;

        public int WorldTick { get => worldTick; }

        private static PlayState playState = PlayState.Stopped;
        private static CaptureState captureState = CaptureState.CaptureActor;

        private FreeCameraRig camera;

        private int worldTick;

        private void Awake()
        {
            instance = this;

            playback = new Playback();
            recorder = new Recorder();

            Cast = new List<ActorPlayer>();
            NPCCast = new List<ActorNPC>();
            WorldProps = new List<ActorProp>();
            RecordingProps = new List<ActorProp>();
        }

        private void Start()
        {
            Events.OnPrePlayback += () => SetPlayState(PlayState.Preplaying);
            Events.OnPreRecord += () => SetPlayState(PlayState.Prerecording);

            Events.OnPlay += () => SetPlayState(PlayState.Playing);
            Events.OnStartRecording += () => SetPlayState(PlayState.Recording);
        }

        private void Update()
        {
            if (!Settings.Debug.useKeys)
            {
                return;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Playback.instance.Seek(-1);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                Playback.instance.Seek(1);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Play();
            }

            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                Record();
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Stop();
            }
        }

        public void Play()
        {
            playback.BeginPlayback();
        }

        public void Pause()
        {
            playState = PlayState.Paused;
        }

        public void Record()
        {
            recorder.BeginRecording();
        }

        public void Retake()
        {
            recorder.RetakeRecording();
            playState = PlayState.Stopped;
            recorder.BeginRecording();
        }

        public void Stop()
        {
            playState = PlayState.Stopped;
        }

        public void SetCamera(FreeCameraRig camera)
        {
            this.camera = camera;
        }

        public void RemoveAllActors()
        {
            playState = PlayState.Stopped;

            for (int i = 0; i < Cast.Count; i++)
            {
                Cast[i].Delete();
            }

            Cast.Clear();
        }
        
        public void ClearScene()
        {
            RemoveAllActors();
            
            foreach(var prop in WorldProps)
            {
                prop.InteractableRigidbody.isKinematic = false;
                GameObject.Destroy(prop);
            }

            WorldProps.Clear();
        }

        private void SetPlayState(PlayState state)
        {
            playState = state;
        }
    }
}
   