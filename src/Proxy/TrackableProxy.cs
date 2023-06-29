using System;
using UnityEngine;

namespace NEP.MonoDirector.Proxy
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class TrackableProxy : MonoBehaviour
    {
        public TrackableProxy(IntPtr ptr) : base(ptr) { }


    }
}
