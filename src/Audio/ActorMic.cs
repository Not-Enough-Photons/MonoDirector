using SLZ.VRMK;
using UnityEngine;

using Avatar = SLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorMic : MonoBehaviour
    {
        public ActorMic(System.IntPtr ptr) : base(ptr) { }

        private Avatar avatar;

        private AudioClip clip;
        private AudioSource source;

        private Transform jaw;

        private Spectrum spectrum;

        private bool micEnabled = true;

        private void Awake()
        {
            source = gameObject.AddComponent<AudioSource>();
            source.spatialBlend = 1f;

            spectrum = gameObject.AddComponent<Spectrum>();
            spectrum.source = source;
            spectrum.freqLow = 0f;
            spectrum.freqHigh = 44100f;
        }

        public void SetAvatar(Avatar avatar)
        {
            this.avatar = avatar;
            jaw = avatar.animator.GetBoneTransform(HumanBodyBones.Jaw);
            transform.parent = jaw;
            transform.localPosition = Vector3.zero;
        }

        public void UpdateJaw()
        {
            jaw.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f + spectrum.BandVol(0f, 44100f) * 10000f));
        }

        public void Playback()
        {
            if(clip != null)
            {
                source.clip = clip;
                source.Play();
            }
        }

        public void StopPlayback()
        {
            source.Stop();
        }

        public void RecordMicrophone()
        {
            if (!micEnabled)
            {
                return;
            }

            clip = Microphone.Start(null, false, 60 * 4, 44100);
        }

        public void StopRecordingMicrophone()
        {
            Microphone.End(null);
        }
    }
}
