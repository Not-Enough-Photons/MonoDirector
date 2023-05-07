 using BoneLib;
using NEP.MonoDirector.Core;
using SLZ.Bonelab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    /// <summary>
    /// Simple camera controller.
    /// </summary>
    /// 
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class FreeCameraRig : MonoBehaviour
    {
        public FreeCameraRig(System.IntPtr ptr) : base(ptr) { }

        public float multiplier = 0.15f;

        public float xFactor = 1f;
        public float yFactor = 1f;

        public Vector3 shakeVector = new Vector3(1.75f, 1.75f, 0.15f);

        private Vector3 wishDir = Vector3.zero;
        private Quaternion rot = Quaternion.identity;
        private float maxLength = 5f;
        private float currentSpeed = 5f;
        private float slowSpeed = 10f;
        private float fastSpeed = 25f;
        private float friction = 5f;
        private float delta => Time.deltaTime;

        private bool fastCamera => Input.GetKey(KeyCode.LeftShift);
        private bool lockCursor => Input.GetMouseButton(1);
        private bool smoothRotation = true;

        private float smoothDamp = 4f;

        private float fovChangeMultiplier = 10f;
        private float fovChangeLerp = 6f;
        private float fovChange = 85f;
        private float lastFov = 0f;

        private Vector3 cameraPosition;

        private Rigidbody rigidbody;

        private InputController inputController = CameraRigManager.Instance.InputController;

        protected void Awake()
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
        }

        protected void Update()
        {
            MoveUpdate();
            MouseUpdate();

            UpdateFOV();
        }

        private void LateUpdate()
        {
            CameraRigManager.Instance.Camera.fieldOfView = Mathf.Lerp(lastFov, fovChange, fovChangeLerp * Time.deltaTime);
        }

        private void UpdateFOV()
        {
            lastFov = CameraRigManager.Instance.Camera.fieldOfView;
            fovChange -= Input.GetAxisRaw("Mouse ScrollWheel") * fovChangeMultiplier;
        }

        private void MouseUpdate()
        {
            Vector3 mouseVector = inputController.MouseMove();

            float shakeX = Mathf.Sin(Time.time) + Mathf.PerlinNoise(xFactor * Time.time, yFactor) * shakeVector.x;
            float shakeY = Mathf.Sin(Time.time) + Mathf.PerlinNoise(xFactor, 1f - yFactor * Time.time) * shakeVector.y;
            float shakeZ = shakeY * shakeVector.z;

            Vector3 rightVector = (Vector3.right * (mouseVector.y + (shakeX * multiplier)));
            Vector3 upVector = (Vector3.up * (mouseVector.x + (shakeY * multiplier)));
            Vector3 forwardVector = (Vector3.forward * (mouseVector.z + (shakeZ * multiplier)));

            transform.rotation = Quaternion.Euler(rightVector + upVector + forwardVector);

            if (!lockCursor)
            {
                return;
            }

            rigidbody.drag = friction;
        }

        private void MoveUpdate()
        {
            Vector3 inputVector = inputController.KeyboardMove();

            Vector3.ClampMagnitude(wishDir, maxLength);

            Transform t = CameraRigManager.Instance.Camera.transform;

            currentSpeed = fastCamera ? fastSpeed : slowSpeed;

            cameraPosition = inputVector * currentSpeed;

            inputVector = Vector3.ClampMagnitude(inputVector, maxLength);

            wishDir = ((t.right * inputVector.x) + (t.up * inputVector.y) + (t.forward * inputVector.z));
            rigidbody.AddForce(wishDir * currentSpeed);
        }
    }
}