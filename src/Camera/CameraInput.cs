using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class CameraInput : MonoBehaviour
    {
        public CameraInput(System.IntPtr ptr) : base(ptr) { }

        private Vector3 wishDir = Vector3.zero;

        private float smoothDamp = 4f;

        private Vector3 cameraInput;
        private Vector3 mouseInput;

        private float maxLength = 5f;
        private float currentSpeed = 5f;
        private float slowSpeed = 10f;
        private float fastSpeed = 25f;
        private float friction = 5f;

        private float xMouseMove = 0f;
        private float yMouseMove = 0f;

        private bool fastCamera => Input.GetKey(KeyCode.LeftShift);
        private bool lockCursor => Input.GetMouseButton(1);
        private bool smoothRotation = true;

        private bool enableKeyboardMovement = false;
        private bool enableMouseMovement = false;
    }
}
