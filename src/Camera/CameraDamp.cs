using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class CameraDamp : MonoBehaviour
    {
        public CameraDamp(System.IntPtr ptr) : base(ptr) { }

        public float delta = 4f;

        private Transform followTarget;

        private Vector3 lastPosition;
        private Quaternion lastRotation;

        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
        }

        private void LateUpdate()
        {
            if(followTarget == null)
            {
                return;
            }

            transform.position = followTarget.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, followTarget.parent.rotation, delta * Time.deltaTime);
        }
    }
}
