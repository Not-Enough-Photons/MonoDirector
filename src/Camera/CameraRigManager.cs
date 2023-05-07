using NEP.MonoDirector.State;

using SLZ.Bonelab;

using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    public class CameraRigManager
    {
        public CameraRigManager(GameObject camera)
        {
            this.cameraObject = camera;
            Start();
        }

        public static CameraRigManager Instance { get; private set; }

        public Camera Camera { get; private set; }

        public FreeCameraRig FreeCamera { get; private set; }
        public FollowCameraRig FollowCamera { get; private set; }

        public CameraDamp CameraDamp { get; private set; }
        public CameraVolume CameraVolume { get; private set; }
        public SmoothFollower SmoothFollower { get; private set; }

        private GameObject cameraObject;
        private CameraMode cameraMode;

        private GameObject cameraModel;
        private MeshRenderer cameraRenderer;

        private void Start()
        {
            Instance = this;

            cameraObject.transform.parent = null;

            InitializeCamera(cameraObject);
            //InitializeCameraModel();
        }

        private void InitializeCamera(GameObject cameraObject)
        {
            cameraObject.transform.parent = null;

            Camera = cameraObject.GetComponent<Camera>();

            SmoothFollower = cameraObject.GetComponent<SmoothFollower>();

            FreeCamera = cameraObject.AddComponent<FreeCameraRig>();
            FollowCamera = cameraObject.AddComponent<FollowCameraRig>();
            CameraDamp = cameraObject.AddComponent<CameraDamp>();
            CameraVolume = cameraObject.AddComponent<CameraVolume>();

            SmoothFollower.enabled = false;

            FreeCamera.enabled = false;
            FollowCamera.enabled = false;

            CameraDamp.enabled = true;
            CameraVolume.enabled = true;

            CameraDamp.SetFollowTarget(SmoothFollower.targetTransform);
        }

        private void InitializeCameraModel()
        {
            cameraModel = GameObject.Instantiate(Main.bundle.LoadAsset("md_camera")).Cast<GameObject>();
            cameraRenderer = cameraModel.transform.Find("geo").GetComponent<MeshRenderer>();

            cameraRenderer.castShadows = false;
            cameraRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

            cameraRenderer.transform.SetParent(cameraObject.transform);
            cameraRenderer.transform.localPosition = Vector3.forward * -0.1f;
            cameraRenderer.transform.eulerAngles = Vector3.zero;
            cameraRenderer.transform.localScale = new Vector3(0.0075f, 0.0075f, -0.0075f);
        }

        public void SetCameraMode(CameraMode cameraMode)
        {
            this.cameraMode = cameraMode;

            if(cameraMode == CameraMode.Free)
            {
                FreeCamera.enabled = true;
                CameraDamp.enabled = false;
            }

            if(cameraMode == CameraMode.Head)
            {
                FreeCamera.enabled = false;
                CameraDamp.enabled = true;
            }
        }
    }
}
