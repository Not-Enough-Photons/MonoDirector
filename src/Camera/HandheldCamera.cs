using NEP.MonoDirector.State;
using SLZ.Interaction;
using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class HandheldCamera : MonoBehaviour
    {
        public HandheldCamera(System.IntPtr ptr) : base(ptr) { }

        private CylinderGrip leftHandle;
        private CylinderGrip rightHandle;

        private Transform gimbal;
        private Transform leftHandleTransform;
        private Transform rightHandleTransform;

        private Camera sensorCamera;

        private GameObject backViewfinderScreen;
        private GameObject frontViewfinderScreen;
        private GameObject displayScreen;

        private RenderTexture displayTexture  => sensorCamera.targetTexture;

        private void Awake()
        {
            gimbal = transform.Find("Gimbal");

            leftHandleTransform = gimbal.Find("Grips/Left Handle");
            rightHandleTransform = gimbal.Find("Grips/Right Handle");

            sensorCamera = gimbal.Find("Sensor").GetComponent<Camera>();
            backViewfinderScreen = gimbal.Find("Studio Camera/Viewfinder_Back").gameObject;
            frontViewfinderScreen = gimbal.Find("Studio Camera/Viewfinder_Front").gameObject;
            displayScreen = gimbal.Find("Studio Camera/Screen").gameObject;

            leftHandle = leftHandleTransform.GetComponent<CylinderGrip>();
            rightHandle = rightHandleTransform.GetComponent<CylinderGrip>();
        }

        private void OnEnable()
        {
            Events.OnCameraModeSet += OnCameraModeChanged;

            leftHandle.attachedUpdateDelegate += new System.Action<Hand>(LeftHandUpdate);
            rightHandle.attachedUpdateDelegate += new System.Action<Hand>(RightHandUpdate);
        }

        private void OnDisable()
        {
            Events.OnCameraModeSet -= OnCameraModeChanged;

            leftHandle.attachedUpdateDelegate -= new System.Action<Hand>(LeftHandUpdate);
            rightHandle.attachedUpdateDelegate -= new System.Action<Hand>(RightHandUpdate);
        }

        private void OnCameraModeChanged(CameraMode mode)
        {
            if(mode == CameraMode.Handheld)
            {
                sensorCamera.enabled = false;
                displayScreen.active = true;
                backViewfinderScreen.active = true;
                frontViewfinderScreen.active = true;

                CameraRigManager.Instance.ClonedCamera.targetTexture = displayTexture;

                CameraRigManager.Instance.ClonedCamera.gameObject.SetActive(true);
                CameraRigManager.Instance.CameraDisplay.FollowCamera.SetFollowTarget(sensorCamera.transform);
                CameraRigManager.Instance.FollowCamera.SetFollowTarget(sensorCamera.transform);
            }
            else
            {
                sensorCamera.enabled = false;
                displayScreen.active = false;
                backViewfinderScreen.active = false;
                frontViewfinderScreen.active = false;

                CameraRigManager.Instance.ClonedCamera.gameObject.SetActive(false);
                CameraRigManager.Instance.FollowCamera.SetDefaultTarget();
            }
        }

        private void LeftHandUpdate(Hand hand)
        {
            if (hand.GetIndexTriggerAxis() > 0.25f)
            {
                float rate = CameraRigManager.Instance.FOVController.fovChangeRate;

                CameraRigManager.Instance.CameraDisplay.FOVController.SetFOV(-(hand.GetIndexTriggerAxis() * rate / 10f));
                CameraRigManager.Instance.FOVController.SetFOV(-(hand.GetIndexTriggerAxis() * rate / 10f));
            }
        }
         
        private void RightHandUpdate(Hand hand)
        {
            if (hand.GetIndexTriggerAxis() > 0.25f)
            {
                float rate = CameraRigManager.Instance.FOVController.fovChangeRate;

                CameraRigManager.Instance.CameraDisplay.FOVController.SetFOV(hand.GetIndexTriggerAxis() * rate / 10f);
                CameraRigManager.Instance.FOVController.SetFOV(hand.GetIndexTriggerAxis() * rate / 10f);
            }
        }
    }
}
