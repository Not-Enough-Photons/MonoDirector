using System;
using UnityEngine;

using NEP.MonoDirector.Actors;

using Avatar = SLZ.VRMK.Avatar;
using System.Collections.Generic;

namespace NEP.MonoDirector.Proxy
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorProxy : TrackableProxy
    {
        public ActorProxy(IntPtr ptr) : base(ptr) { }
    }
}
