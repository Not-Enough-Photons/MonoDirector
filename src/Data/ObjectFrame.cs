using UnityEngine;

namespace NEP.MonoDirector.Data
{
    public struct ObjectFrame
    {
        public ObjectFrame(Vector3 position)
        {
            name = "Bone";
            this.transform = null;
            this.position = position;
            this.rotation = Quaternion.identity;
            this.scale = Vector3.one;

            frameTime = 0f;

            rigidbody = null;
            rigidbodyVelocity = Vector3.zero;
            rigidbodyAngularVelocity = Vector3.zero;
        }

        public ObjectFrame(Vector3 position, Quaternion rotation)
        {
            name = "Bone";
            this.transform = null;
            this.position = position;
            this.rotation = rotation;
            this.scale = Vector3.one;

            frameTime = 0f;

            rigidbody = null;
            rigidbodyVelocity = Vector3.zero;
            rigidbodyAngularVelocity = Vector3.zero;
        }

        public ObjectFrame(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            name = "Bone";
            this.transform = null;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;

            frameTime = 0f;

            rigidbody = null;
            rigidbodyVelocity = Vector3.zero;
            rigidbodyAngularVelocity = Vector3.zero;
        }

        public ObjectFrame(Quaternion rotation)
        {
            name = "Bone";
            this.transform = null;
            this.position = Vector3.zero;
            this.rotation = rotation;
            this.scale = Vector3.one;

            frameTime = 0f;

            rigidbody = null;
            rigidbodyVelocity = Vector3.zero;
            rigidbodyAngularVelocity = Vector3.zero;
        }

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
