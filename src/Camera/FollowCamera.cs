using NEP.MonoDirector.State;
using System.Collections.Generic;
using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class FollowCamera : MonoBehaviour
    {
        public FollowCamera(System.IntPtr ptr) : base(ptr) { }

        public readonly Dictionary<BodyPart, Vector3> FollowPoints = new Dictionary<BodyPart, Vector3>()
        {
            { BodyPart.Head, new Vector3() },
            { BodyPart.Chest, new Vector3() },
            { BodyPart.Pelvis, new Vector3() }
        };

        public Transform FollowTarget { get => followTarget; }

        public float delta = 4f;

        private Vector3 positionOffset;
        private Vector3 rotationEulerOffset;

        private Transform followTarget;

        protected void Update()
        {
            if(followTarget == null)
            {
                return;
            }

            transform.position = followTarget.position;
        }

        private void LateUpdate()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, followTarget.rotation, delta * Time.deltaTime);
        }

        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
        }

        public void SetPositionOffset(Vector3 offset)
        {
            positionOffset = offset;
        }

        public void SetRotationOffset(Vector3 offset)
        {
            rotationEulerOffset = offset;
        }

        public void SetRotationOffset(Quaternion offset)
        {
            rotationEulerOffset = offset.eulerAngles;
        }

        public void SetFollowBone(BodyPart part)
        {
            positionOffset = Vector3.zero;
            rotationEulerOffset = Vector3.zero;

            Vector3 point = FollowPoints[part];

            followTarget.position = point;

            followTarget.localPosition = positionOffset;
            followTarget.localRotation = Quaternion.Euler(rotationEulerOffset);
        }
    }
}
