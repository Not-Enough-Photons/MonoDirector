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
            Events.OnPreRecord += Preroll;
            Events.OnStopRecording += Beep;

            Events.OnPrePlayback += Postroll;
            Events.OnStopPlayback += Beep;

            Events.OnPreSnapshot += Preroll;

            Events.OnTimerCountdown += Beep;
        }

        private void OnDisable()
        {
            Events.OnStartRecording -= Preroll;
            Events.OnStopRecording -= Beep;

            Events.OnPlay -= Postroll;
            Events.OnStopPlayback -= Beep;

            Events.OnPreSnapshot -= Preroll;

            Events.OnTimerCountdown -= Beep;
        }

        public void Play(AudioClip clip)
        {
            source.clip = clip;
            source.PlayOneShot(clip);
        }

        private void Preroll()
        {
            Play(sfx_preroll);
        }

        private void Beep()
        {
            Play(sfx_beep);
        }

        private void Postroll() 
        {
            Play(sfx_postroll);
        }
    }
}
