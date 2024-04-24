using System.Collections.Generic;
using NEP.MonoDirector.Data;
using SLZ.Marrow.Pool;
using UnityEngine;
using UnityEngine.Rendering.Universal.LibTessDotNet;

namespace NEP.MonoDirector.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SoundHolder : MonoBehaviour
    {
        public SoundHolder(System.IntPtr ptr) : base(ptr) { }

        public static Dictionary<string, AudioClip> LoadedClips;

        private AssetPoolee poolee;

        private AudioClip sound;

        private void Start()
        {
            poolee = GetComponent<AssetPoolee>();
            AssignSound(WarehouseLoader.soundTable[poolee.spawnableCrate.Description]);
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