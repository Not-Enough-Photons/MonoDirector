using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class FOVController : MonoBehaviour
    {
        public FOVController(System.IntPtr ptr) : base(ptr) { }

        public float fovChangeSmoothing = 10f;
        public float fovChangeRate = 4f;

        private Camera camera => CameraRigManager.Instance.Camera;

        private float fieldOfView = 90f;
        private float lastFieldOfView = 0f;

        private void Update()
        {
            UpdateFOV();
        }

        private void LateUpdate()
        {
            camera.fieldOfView = Mathf.Lerp(lastFieldOfView, fieldOfView, fovChangeSmoothing * Time.deltaTime);
        }

        private void UpdateFOV()
        {
            lastFieldOfView = camera.fieldOfView;
            fieldOfView -= Input.GetAxisRaw("Mouse ScrollWheel") * fovChangeRate;
        }
    }
}
