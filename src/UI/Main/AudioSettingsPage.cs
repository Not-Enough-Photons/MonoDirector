using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class AudioSettingsPage : MonoBehaviour
    {
        public AudioSettingsPage(System.IntPtr ptr) : base(ptr) { }

        private Transform grid => transform.Find("Grid");

        private GameObject enableMicObject => grid.Find("EnableMic").gameObject;
        private GameObject enableMicPlaybackObject => grid.Find("EnableMicPlayback").gameObject;
        private GameObject micVolumeObject => grid.Find("MicVolume").gameObject;

        private Toggle enableMic => enableMicObject.GetComponentInChildren<Toggle>();
        private Toggle enableMicPlayback => enableMicPlaybackObject.GetComponentInChildren<Toggle>();
    }
}

