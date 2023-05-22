using UnityEngine;
using NEP.MonoDirector.Core;

using TMPro;
using NEP.MonoDirector.State;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class InformationInterface : MonoBehaviour
    {
        public InformationInterface(System.IntPtr ptr) : base(ptr) { }

        public static InformationInterface Instance { get; private set; }

        public bool ShowUI
        {
            get
            {
                return showUI;
            }
            set
            {
                showUI = value;

                ShowIcons = showUI;
                ShowTimecode = showUI;
                ShowPlaymode = showUI;
            }
        }

        public bool ShowIcons
        {
            get
            {
                return showIcons;
            }
            set
            {
                showIcons = value;
                microIconsObject?.SetActive(showIcons);
            }
        }

        public bool ShowTimecode
        {
            get
            {
                return showTimecode;
            }
            set
            {
                showTimecode = value;
                timecodeObject?.SetActive(showTimecode);
            }
        }

        public bool ShowPlaymode
        {
            get
            {
                return showPlaymode;
            }
            set
            {
                showPlaymode = value;
                playmodeObject?.SetActive(showPlaymode);
            }
        }

        private GameObject microIconsObject;
        private GameObject timecodeObject;
        private GameObject playmodeObject;

        private GameObject micObject;
        private GameObject micOffObject;

        private TextMeshProUGUI timecodeText;
        private TextMeshProUGUI playmodeText;

        private PlayState playState;

        private bool showUI;
        private bool showIcons;
        private bool showTimecode;
        private bool showPlaymode;

        private void Awake()
        {
            Instance = this;

            microIconsObject = transform.Find("MicroIcons").gameObject;
            timecodeObject = transform.Find("Timecode").gameObject;
            playmodeObject = transform.Find("Playmode").gameObject;

            micObject = microIconsObject.transform.Find("Microphone/Mic").gameObject;
            micOffObject = microIconsObject.transform.Find("Microphone/Disabled").gameObject;

            timecodeText = timecodeObject.transform.Find("Time").GetComponent<TextMeshProUGUI>();
            playmodeText = playmodeObject.transform.Find("Mode").GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            Events.OnPlayStateSet += OnPlayStateSet;

            Events.OnPrePlayback += OnSceneStart;
            Events.OnPlaybackTick += OnSceneTick;
            Events.OnStopPlayback += OnSceneEnd;

            Events.OnPreRecord += OnSceneStart;
            Events.OnRecordTick += OnSceneTick;
            Events.OnStopRecording += OnSceneEnd;

            showIcons = false;
            showTimecode = false;
            showPlaymode = false;

            microIconsObject.SetActive(false);
            timecodeObject.SetActive(false);
            playmodeObject.SetActive(false);
        }

        private void Update()
        {
            micOffObject.SetActive(!Settings.World.useMicrophone);

            transform.position = Vector3.Lerp(transform.position, BoneLib.Player.playerHead.position + BoneLib.Player.playerHead.forward, 16f * Time.deltaTime);
            transform.LookAt(BoneLib.Player.playerHead);
        }

        public void OnSceneStart()
        {
            timecodeText.text = "0s";
        }

        public void OnSceneTick()
        {
            float time = 0f;

            if(playState == PlayState.Playing)
            {
                time = Playback.instance.PlaybackTime;
            }

            if(playState == PlayState.Recording)
            {
                time = Recorder.instance.RecordingTime;
            }

            timecodeText.text = time.ToString("0.000") + "s";
        }

        public void OnSceneEnd()
        {
            
        }

        private void OnPlayStateSet(PlayState playState)
        {
            this.playState = playState;
            playmodeText.text = playState.ToString();
        }
    }
}
