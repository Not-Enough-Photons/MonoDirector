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

        private Transform head;
        private Transform jaw;

        private Vector3 initialJawRotation;

        private Spectrum spectrum;

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

            head = avatar.animator.GetBoneTransform(HumanBodyBones.Head);
            jaw = avatar.animator.GetBoneTransform(HumanBodyBones.Jaw);

            initialJawRotation = new Vector3(jaw.localEulerAngles.x, jaw.localEulerAngles.y, jaw.localEulerAngles.z);

            transform.parent = jaw != null ? jaw : head;
            transform.localPosition = Vector3.zero;
        }

        public void UpdateJaw()
        {
            if(jaw == null)
            {
                return;
            }

            jaw.localRotation = Quaternion.Euler(new Vector3(initialJawRotation.x, initialJawRotation.y, initialJawRotation.z + spectrum.BandVol(0f, 44100f) * 10000f));
        }

        public void Playback()
        {
            if (!Settings.World.micPlayback)
            {
                return;
            }

            if (clip != null)
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
            if (!Settings.World.useMicrophone)
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
