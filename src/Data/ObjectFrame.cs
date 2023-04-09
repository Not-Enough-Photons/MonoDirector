using System;
using UnityEngine;

namespace NEP.MonoDirector.Data
{
    public class ObjectFrame
    {
        public ObjectFrame(Transform transform)
        {
            name = transform != null ? transform.name : "Null";

            deltaTime = 0f;

            this.transform = transform;
            position = transform != null ? transform.position : Vector3.zero;
            rotation = transform != null ? transform.rotation : Quaternion.identity;
            scale = transform != null ? transform.localScale : Vector3.one;

            rigidbody = null;
            rigidbodyVelocity = Vector3.zero;
            rigidbodyAngularVelocity = Vector3.zero;

            previous = null;
            next = null;
        }

        public ObjectFrame(Rigidbody rigidbody)
        {
            name = rigidbody.name;

            deltaTime = 0f;

            this.transform = rigidbody.transform;
            position = transform != null ? transform.position : Vector3.zero;
            rotation = transform != null ? transform.rotation : Quaternion.identity;
            scale = transform != null ? transform.localScale : Vector3.one;

            this.rigidbody = rigidbody;
            rigidbodyVelocity = rigidbody.velocity;
            rigidbodyAngularVelocity = rigidbody.angularVelocity;

            previous = null;
            next = null;
        }

        public ObjectFrame previous;
        public ObjectFrame next;

        public string name;

        public float deltaTime;

        public Transform transform;
        public Rigidbody rigidbody;

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public Vector3 rigidbodyVelocity;
        public Vector3 rigidbodyAngularVelocity;

        public void SetDelta(float deltaTime)
        {
            this.deltaTime = deltaTime;
        }
    }
}
