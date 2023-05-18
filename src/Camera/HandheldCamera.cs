using SLZ.Interaction;
using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class HandheldCamera : MonoBehaviour
    {
        public HandheldCamera(System.IntPtr ptr) : base(ptr) { }

        public Transform gimbal
        {
            get
            {
                if(_gimbal == null)
                {
                    _gimbal = transform.Find("Gimbal");
                }

                return _gimbal;
            }
        }

        public Transform leftHandleTransform
        {
            get
            {
                if(_leftHandleTransform == null)
                {
                    _leftHandleTransform = gimbal.Find("Grips/Left Handle");
                }

                return _leftHandleTransform;
            }
        }

        public Transform rightHandleTransform
        {
            get
            {
                if(_rightHandleTransform == null)
                {
                    _rightHandleTransform = gimbal.Find("Grips/Right Handle");
                }

                return _rightHandleTransform;
            }
        }

        public CylinderGrip leftHandle { get => leftHandleTransform.GetComponent<CylinderGrip>(); }
        public CylinderGrip rightHandle { get => rightHandleTransform.GetComponent<CylinderGrip>(); }

        public Camera sensorCamera
        {
            get
            {
                if(_sensorCamera == null)
                {
                    _sensorCamera = gimbal.Find("Sensor").GetComponent<Camera>();
                }

                return _sensorCamera;
            }
        }

        public GameObject viewfinderScreen
        {
            get
            {
                if(_viewfinderScreen == null)
                {
                    _viewfinderScreen = gimbal.Find("Studio Camera/Viewfinder").gameObject;
                }

                return _viewfinderScreen;
            }
        }
        
        public GameObject displayScreen
        {
            get
            {
                if (_displayScreen == null)
                {
                    _displayScreen = gimbal.Find("Studio Camera/Screen").gameObject;
                }

                return _displayScreen;
            }
        }

        private Transform _gimbal;
        private Transform _leftHandleTransform;
        private Transform _rightHandleTransform;

        private Camera _sensorCamera;

        private GameObject _viewfinderScreen;
        private GameObject _displayScreen;

        private RenderTexture _displayTexture { get => sensorCamera.targetTexture; }

        private void Start()
        {
            CameraRigManager.Instance.ClonedCamera.nearClipPlane = 0.25f;
        }

        private void OnEnable()
        {
            leftHandle.attachedUpdateDelegate += new System.Action<Hand>(LeftHandUpdate);
            rightHandle.attachedUpdateDelegate += new System.Action<Hand>(RightHandUpdate);

            leftHandle.attachedHandDelegate += new System.Action<Hand>(LeftHandAttached);
            rightHandle.attachedHandDelegate += new System.Action<Hand>(RightHandAttached);

            leftHandle.detachedHandDelegate += new System.Action<Hand>(LeftHandDetached);
            rightHandle.detachedHandDelegate += new System.Action<Hand>(RightHandDetached);
        }

        private void OnDisable()
        {
            leftHandle.attachedUpdateDelegate -= new System.Action<Hand>(LeftHandUpdate);
            rightHandle.attachedUpdateDelegate -= new System.Action<Hand>(RightHandUpdate);

            leftHandle.attachedHandDelegate -= new System.Action<Hand>(LeftHandAttached);
            rightHandle.attachedHandDelegate -= new System.Action<Hand>(RightHandAttached);

            leftHandle.detachedHandDelegate -= new System.Action<Hand>(LeftHandDetached);
            rightHandle.detachedHandDelegate -= new System.Action<Hand>(RightHandDetached);
        }

        private void LeftHandUpdate(Hand hand)
        {
            if (hand._indexButton)
            {
                float rate = CameraRigManager.Instance.FOVController.fovChangeRate;

                CameraRigManager.Instance.CameraDisplay.FOVController.SetFOV(-(rate * Time.deltaTime));
                CameraRigManager.Instance.FOVController.SetFOV(-(rate * Time.deltaTime));
            }
        }

        private void RightHandUpdate(Hand hand)
        {
            if (hand._indexButton)
            {
                float rate = CameraRigManager.Instance.FOVController.fovChangeRate;

                CameraRigManager.Instance.CameraDisplay.FOVController.SetFOV(rate * Time.deltaTime);
                CameraRigManager.Instance.FOVController.SetFOV(rate * Time.deltaTime);
            }
        }

        private void LeftHandAttached(Hand hand)
        {
            sensorCamera.enabled = false;
            displayScreen.active = true;
            viewfinderScreen.active = true;
            
            CameraRigManager.Instance.ClonedCamera.targetTexture = _displayTexture;

            CameraRigManager.Instance.ClonedCamera.gameObject.SetActive(true);
            CameraRigManager.Instance.CameraDisplay.FollowCamera.SetFollowTarget(sensorCamera.transform);
            CameraRigManager.Instance.FollowCamera.SetFollowTarget(sensorCamera.transform);
            CameraRigManager.Instance.Camera.nearClipPlane = 0.25f;
        }
         
        private void RightHandAttached(Hand hand)
        {
            LeftHandAttached(hand);
        }

        private void LeftHandDetached(Hand hand)
        {
            sensorCamera.enabled = false;
            displayScreen.active = false;
            viewfinderScreen.active = false;

            CameraRigManager.Instance.ClonedCamera.gameObject.SetActive(false);
            CameraRigManager.Instance.FollowCamera.SetDefaultTarget();
            CameraRigManager.Instance.Camera.nearClipPlane = 0.01f;
        }

        private void RightHandDetached(Hand hand)
        {
            LeftHandDetached(hand);
        }
    }
}
