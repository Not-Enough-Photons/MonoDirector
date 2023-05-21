using UnityEngine;
using NEP.MonoDirector.Core;

using TMPro;
using NEP.MonoDirector.State;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class Timecode : MonoBehaviour
    {
        public Timecode(System.IntPtr ptr) : base(ptr) { }

        public static Timecode Instance { get; private set; }

        public bool ShowTimecode
        {
            get
            {
                return showTimecode;
            }
            set
            {
                showTimecode = value;
                gameObject.SetActive(showTimecode);
            }
        }

        private TextMeshProUGUI timecodeText;

        private PlayState playState;

        private bool showTimecode;

        private void Awake()
        {
            Instance = this;
            timecodeText = transform.Find("Time").GetComponent<TextMeshProUGUI>();
            ShowTimecode = false;
            gameObject.SetActive(false);
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
        }

        private void Update()
        {
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
        }
    }
}
