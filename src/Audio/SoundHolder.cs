using NEP.MonoDirector.Data;
using UnityEngine;

namespace NEP.MonoDirector.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SoundHolder : MonoBehaviour
    {
        public SoundHolder(System.IntPtr ptr) : base(ptr) { }

        private AudioClip sound;

        private void Awake()
        {
            var sounds = WarehouseLoader.GetSounds();
            sound = sounds[0];
        }

        public void AssignSound(AudioClip sound)
        {
            this.sound = sound;
        }

        public AudioClip GetSound()
        {
            return sound;
        }
    }
}