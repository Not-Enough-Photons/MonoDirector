using AudioImportLib;
using MelonLoader;
using UnityEngine;

namespace NEP.MonoDirector.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class FeedbackSFX : MonoBehaviour
    {
        public FeedbackSFX(System.IntPtr ptr) : base(ptr) { }

        private AudioSource source;

        private AudioClip sfx_preroll = API.LoadAudioClip(Constants.dirSFX + "preroll.wav");
        private AudioClip sfx_postroll = API.LoadAudioClip(Constants.dirSFX + "postroll.wav");
        private AudioClip sfx_beep = API.LoadAudioClip(Constants.dirSFX + "beep.wav");

        private void Awake()
        {
            source = gameObject.AddComponent<AudioSource>();
        }

        private void OnEnable()
        {
            Events.OnPreRecord += OnStartRecording;
            Events.OnStopRecording += OnStopRecording;

            Events.OnPrePlayback += OnPrePlay;
            Events.OnStopPlayback += OnStopPlayback;

            Events.OnPreSnapshot += OnStartRecording;
        }

        private void OnDisable()
        {
            Events.OnStartRecording -= OnStartRecording;
            Events.OnStopRecording -= OnStopRecording;

            Events.OnPlay -= OnPrePlay;
            Events.OnStopPlayback -= OnStopPlayback;

            Events.OnPreSnapshot -= OnStartRecording;
        }

        public void Play(AudioClip clip)
        {
            source.clip = clip;
            source.PlayOneShot(clip);
        }

        private void OnStartRecording()
        {
            Play(sfx_preroll);
        }

        private void OnStopRecording()
        {
            Play(sfx_beep);
        }

        private void OnPrePlay() 
        {
            Play(sfx_postroll);
        }

        private void OnStopPlayback()
        {
            Play(sfx_beep);
        }
    }
}
