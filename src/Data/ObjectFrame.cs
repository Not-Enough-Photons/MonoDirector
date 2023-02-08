using System;
using UnityEngine;

namespace NEP.MonoDirector.Data
{
    public struct ObjectFrame
    {
        public ObjectFrame(Transform transform)
        {
            this.transform = transform;
            position = transform != null ? transform.position : Vector3.zero;
            rotation = transform != null ? transform.rotation : Quaternion.identity;
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
        }

        public Transform transform;
        public Vector3 position;
        public Quaternion rotation;
    }
}
