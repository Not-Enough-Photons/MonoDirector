using NEP.MonoDirector.Cameras;
using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class FollowCameraRig : MonoBehaviour
    {
        public FollowCameraRig(System.IntPtr ptr) : base(ptr) { }

        public Transform FollowTarget { get => followTarget; }

        private Transform followTarget;

        protected void Update()
        {
            if(followTarget == null)
            {
                return;
            }

            transform.rotation = Quaternion.LookRotation(followTarget.position - transform.position);
        }

        public void SetFollowTransform(Transform followTarget)
        {
            this.followTarget = followTarget;
        }
    }
}
