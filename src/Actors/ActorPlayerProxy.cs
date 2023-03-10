using Il2CppSystem.Collections.Generic;
using NEP.MonoDirector.Data;
using System;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorPlayerProxy : MonoBehaviour
    {
        public ActorPlayerProxy(IntPtr ptr) : base(ptr) { }

        public Avatar Avatar { get => avatar; }

        private Avatar avatar;

        public void CloneAvatar() { }

        public void RecordFrame()
        {

        }

        private List<ObjectFrame> CaptureBoneFrames(Transform[] boneList)
        {
            return null;
        }

        private Transform[] GetAvatarBones(Avatar avatar)
        {
            return null;
        }
    }
}
