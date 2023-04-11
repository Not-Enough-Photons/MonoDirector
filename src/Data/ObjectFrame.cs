using System;
using UnityEngine;
using static SLZ.VFX.GenericFrameTimer;

namespace NEP.MonoDirector.Data
{
    public struct ObjectFrame
    {
        public ObjectFrame(Transform transform)
        {
            name = transform != null ? transform.name : "Null";

            this.transform = transform;
            position = transform != null ? transform.position : Vector3.zero;
            rotation = transform != null ? transform.rotation : Quaternion.identity;
            scale = transform != null ? transform.localScale : Vector3.one;

            frameTime = 0f;

            rigidbody = null;
            rigidbodyVelocity = Vector3.zero;
            rigidbodyAngularVelocity = Vector3.zero;
        }

        public ObjectFrame(Rigidbody rigidbody)
        {
            name = rigidbody.name;

            this.transform = rigidbody.transform;
            position = transform != null ? transform.position : Vector3.zero;
            rotation = transform != null ? transform.rotation : Quaternion.identity;
            scale = transform != null ? transform.localScale : Vector3.one;

            frameTime = 0f;

            this.rigidbody = rigidbody;
            rigidbodyVelocity = rigidbody.velocity;
            rigidbodyAngularVelocity = rigidbody.angularVelocity;
        }

        public void SetDelta(float frameTime)
        {
            this.frameTime = frameTime;
        }

        public string name;

        public Transform transform;
        public Rigidbody rigidbody;

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public float frameTime;

        public Vector3 rigidbodyVelocity;
        public Vector3 rigidbodyAngularVelocity;
    }
}
