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

        private Rigidbody cameraRigidbody;

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

            cameraRigidbody = transform.GetChild(0).GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            Events.OnCameraModeSet += OnCameraModeChanged;

            leftHandle.attachedUpdateDelegate += new System.Action<Hand>(LeftHandUpdate);
            rightHandle.attachedUpdateDelegate += new System.Action<Hand>(RightHandUpdate);
            leftHandle.detachedHandDelegate += new System.Action<Hand>(LeftHandDetached);
        }

        private void OnDisable()
        {
            Events.OnCameraModeSet -= OnCameraModeChanged;

            leftHandle.attachedUpdateDelegate -= new System.Action<Hand>(LeftHandUpdate);
            rightHandle.attachedUpdateDelegate -= new System.Action<Hand>(RightHandUpdate);
        }

        private void Update()
        {
            // I don't feel like making more actions that get called when you update a BoneMenu setting
            // So here's some hack about locking rotations using bit shifting and dividing 

            int bitLockX = Settings.Camera.handheldLockXAxis ? 1 : 0;
            int bitLockY = Settings.Camera.handheldLockYAxis ? 1 : 0;
            int bitLockZ = Settings.Camera.handheldLockZAxis ? 1 : 0;

            RigidbodyConstraints constraintX = (RigidbodyConstraints)(16 * bitLockX);
            RigidbodyConstraints constraintY = (RigidbodyConstraints)(32 * bitLockY);
            RigidbodyConstraints constraintZ = (RigidbodyConstraints)(64 * bitLockZ);

            cameraRigidbody.constraints = constraintX;
            cameraRigidbody.constraints = constraintY;
            cameraRigidbody.constraints = constraintZ;
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
            cameraRigidbody.isKinematic = false;

            if (hand.GetIndexTriggerAxis() > 0.25f)
            {
                float rate = CameraRigManager.Instance.FOVController.fovChangeRate;

                CameraRigManager.Instance.CameraDisplay.FOVController.SetFOV(-(hand.GetIndexTriggerAxis() * rate / 10f));
                CameraRigManager.Instance.FOVController.SetFOV(-(hand.GetIndexTriggerAxis() * rate / 10f));
            }
        }
         
        private void RightHandUpdate(Hand hand)
        {
            cameraRigidbody.isKinematic = false;

            if (hand.GetIndexTriggerAxis() > 0.25f)
            {
                float rate = CameraRigManager.Instance.FOVController.fovChangeRate;

                CameraRigManager.Instance.CameraDisplay.FOVController.SetFOV(hand.GetIndexTriggerAxis() * rate / 10f);
                CameraRigManager.Instance.FOVController.SetFOV(hand.GetIndexTriggerAxis() * rate / 10f);
            }
        }

        private void LeftHandDetached(Hand hand)
        {
            if (Settings.Camera.handheldKinematicOnRelease)
            {
                cameraRigidbody.isKinematic = true;
            }
        }
    }
}
