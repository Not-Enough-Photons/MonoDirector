using NEP.MonoDirector.State;
using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class CameraDamp : MonoBehaviour
    {
        public CameraDamp(System.IntPtr ptr) : base(ptr) { }

        public float delta = 4f;

        private Transform followTarget;

        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
        }

        public void SetFollowBone(BodyPart part)
        {
            transform.localPosition = Vector3.zero;

            switch (part)
            {
                case BodyPart.Head:
                    SetFollowTarget(Constants.rigManager.ControllerRig.m_head);
                    break; 
                case BodyPart.Chest:
                    SetFollowTarget(Constants.rigManager.ControllerRig.m_chest);
                    break;
                case BodyPart.Pelvis:
                    SetFollowTarget(Constants.rigManager.ControllerRig.m_pelvis);
                    break;
                case BodyPart.LeftHand:
                    SetFollowTarget(Constants.rigManager.ControllerRig.m_handLf);
                    break;
                case BodyPart.RightHand:
                    SetFollowTarget(Constants.rigManager.ControllerRig.m_handRt);
                    break;
            }
        }

        private void LateUpdate()
        {
            if(followTarget == null)
            {
                return;
            }

            transform.position = followTarget.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, followTarget.rotation, delta * Time.deltaTime);
        }
    }
}
