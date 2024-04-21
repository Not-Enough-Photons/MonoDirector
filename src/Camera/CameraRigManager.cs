using NEP.MonoDirector.Patches;
using NEP.MonoDirector.State;

using SLZ.Bonelab;

using UnityEngine;

using RigManager = SLZ.Rig.RigManager;

namespace NEP.MonoDirector.Cameras
{
    public class CameraRigManager
    {
        public CameraRigManager()
        {
            RigManager rigManager = BoneLib.Player.rigManager;
            RigScreenOptions screenOptions = rigManager.GetComponent<RigScreenOptions>();
            RigScreenOptions = screenOptions;
            Start();
        }

        public static CameraRigManager Instance { get; private set; }

        public RigScreenOptions RigScreenOptions { get; private set; }

        public Camera Camera { get; private set; }
        public Camera ClonedCamera { get; private set; }

        public InputController InputController { get; private set; }
        public FreeCamera FreeCamera { get; private set; }
        public FollowCamera FollowCamera { get; private set; }

        public FOVController FOVController { get; private set; }
        public CameraDamp CameraDamp { get; private set; }
        public CameraVolume CameraVolume { get; private set; }
        public CameraDisplay CameraDisplay { get; private set; }
        public SmoothFollower SmoothFollower { get; private set; }

        public RenderTexture CameraDisplayTexture { get; private set; }

        public CameraMode CameraMode
        {
            get => cameraMode;
            set
            {
                cameraMode = value;

                // Default spectator camera mode
                if (cameraMode == CameraMode.None)
                {
                    // Disable any effects that we have on the camera
                    FreeCamera.enabled = false;
                    CameraDamp.enabled = false;
                    FollowCamera.enabled = false;

                    // Override any effects
                    SmoothFollower.enabled = true;
                }

                // Free camera mode using WASD and the mouse
                if (cameraMode == CameraMode.Free)
                {
                    SmoothFollower.enabled = false;

                    FreeCamera.enabled = true;

                    CameraDamp.enabled = false;
                    FollowCamera.enabled = false;
                }

                // Modified spectator camera with smooth rotations and custom targets
                if (cameraMode == CameraMode.Head)
                {
                    SmoothFollower.enabled = false;

                    CameraDamp.enabled = false;
                    FollowCamera.enabled = true;

                    FreeCamera.enabled = false;
                }

                Events.OnCameraModeSet?.Invoke(cameraMode);
            }
        }

        public float CameraSmoothness
        {
            get => FollowCamera.delta;
            set => FollowCamera.delta = value;
        }

        public float MouseSensitivity
        {
            get => InputController.mouseSensitivity;
            set => InputController.mouseSensitivity = value;
        }
        
        public float MouseSmoothness
        {
            get => InputController.mouseSmoothness;
            set => InputController.mouseSmoothness = value;
        }

        public float SlowSpeed
        {
            get => FreeCamera.CameraSettings.slowSpeed;
            set => FreeCamera.CameraSettings.slowSpeed = value;
        }

        public float FastSpeed
        {
            get => FreeCamera.CameraSettings.fastSpeed;
            set => FreeCamera.CameraSettings.fastSpeed = value;
        }

        public float MaxSpeed
        {
            get => FreeCamera.CameraSettings.maxSpeed;
            set => FreeCamera.CameraSettings.maxSpeed = value;
        }

        public float Friction
        {
            get => FreeCamera.CameraSettings.friction;
            set => FreeCamera.CameraSettings.friction = value;
        }

        private GameObject cameraObject;
        private CameraMode cameraMode;

        private GameObject cameraModel;
        private MeshRenderer cameraRenderer;

        private void Start()
        {
            Instance = this;

            InitializeCamera(RigScreenOptions);
            //InitializeCameraModel();
        }

        private void InitializeCamera(RigScreenOptions screenOptions)
        {
            Camera = screenOptions.cam;
            cameraObject = Camera.gameObject;

            ClonedCamera = GameObject.Instantiate(cameraObject).GetComponent<Camera>();
            ClonedCamera.gameObject.SetActive(false);

            cameraObject.transform.parent = null;

            SmoothFollower = cameraObject.GetComponent<SmoothFollower>();
            InputController = cameraObject.AddComponent<InputController>();

            FreeCamera = cameraObject.AddComponent<FreeCamera>();

            FOVController = cameraObject.AddComponent<FOVController>();
            FollowCamera = cameraObject.AddComponent<FollowCamera>();
            CameraDamp = cameraObject.AddComponent<CameraDamp>();
            CameraVolume = cameraObject.AddComponent<CameraVolume>();
            //CameraDisplay = cameraObject.AddComponent<CameraDisplay>();

            FOVController.SetCamera(Camera);

            CameraMode = CameraMode.None;

            FollowCamera.SetFollowTarget(SmoothFollower.targetTransform);

            CameraDisplay = ClonedCamera.gameObject.AddComponent<CameraDisplay>();
        }

        private void InitializeCameraModel()
        {
            // cameraModel = GameObject.Instantiate(Main.Bundle.LoadAsset("md_camera")).Cast<GameObject>();
            cameraRenderer = cameraModel.transform.Find("geo").GetComponent<MeshRenderer>();

            cameraRenderer.castShadows = false;
            cameraRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

            cameraRenderer.transform.SetParent(cameraObject.transform);
            cameraRenderer.transform.localPosition = Vector3.forward * -0.1f;
            cameraRenderer.transform.eulerAngles = Vector3.zero;
            cameraRenderer.transform.localScale = new Vector3(0.0075f, 0.0075f, -0.0075f);
        }
    }
}
