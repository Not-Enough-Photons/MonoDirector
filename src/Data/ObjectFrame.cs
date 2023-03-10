using System;
using UnityEngine;

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

            this.rigidbody = rigidbody;
            rigidbodyVelocity = rigidbody.velocity;
            rigidbodyAngularVelocity = rigidbody.angularVelocity;
        }

        public string name;

        public Transform transform;
        public Rigidbody rigidbody;

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public Vector3 rigidbodyVelocity;
        public Vector3 rigidbodyAngularVelocity;
    }
}
