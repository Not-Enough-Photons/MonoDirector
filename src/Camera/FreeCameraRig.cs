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

        private bool enableKeyboardMovement = false;
        private bool enableMouseMovement = false;

        private float smoothDamp = 4f;

        private float fovChangeMultiplier = 10f;
        private float fovChangeLerp = 6f;
        private float fovChange = 85f;
        private float lastFov = 0f;

        private Vector3 cameraPosition;
        private Vector3 cameraInput;
        private Vector3 mouseInput;

        private float xMouseMove = 0f;
        private float yMouseMove = 0f;

        private Rigidbody rigidbody;

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

        private void OnEnable()
        {
            enableKeyboardMovement = true;
            enableMouseMovement = true;
        }

        private void OnDisable()
        {
            enableKeyboardMovement = false;
            enableMouseMovement = false;
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
            if (!enableMouseMovement)
            {
                return;
            }

            if (xMouseMove > 0f || xMouseMove < 0f)
            {
                xMouseMove = Mathf.Lerp(xMouseMove, 0f, smoothDamp * Time.deltaTime);
            }

            if (yMouseMove > 0f || yMouseMove < 0f)
            {
                yMouseMove = Mathf.Lerp(yMouseMove, 0f, smoothDamp * Time.deltaTime);
            }

            float shakeX = Mathf.Sin(Time.time) + Mathf.PerlinNoise(xFactor * Time.time, yFactor) * shakeVector.x;
            float shakeY = Mathf.Sin(Time.time) + Mathf.PerlinNoise(xFactor, 1f - yFactor * Time.time) * shakeVector.y;
            float shakeZ = shakeY * shakeVector.z;

            Vector3 rightVector = (Vector3.right * (mouseInput.y + (shakeX * multiplier)));
            Vector3 upVector = (Vector3.up * (mouseInput.x + (shakeY * multiplier)));
            Vector3 forwardVector = (Vector3.forward * (mouseInput.z + (shakeZ * multiplier)));

            transform.rotation = Quaternion.Euler(rightVector + upVector + forwardVector);

            if (!lockCursor)
            {
                return;
            }

            rigidbody.drag = friction;

            float x = Input.GetAxisRaw("Mouse X");
            float y = Input.GetAxisRaw("Mouse Y");

            bool rollCam = Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(1);

            if (rollCam)
            {
                mouseInput.z -= x;
            }

            if (smoothRotation)
            {
                xMouseMove += x * delta;
                yMouseMove += y * delta;

                mouseInput.x += xMouseMove;
                mouseInput.y -= yMouseMove;
            }
            else
            {
                mouseInput.x += x;
                mouseInput.y -= y;
            }
        }

        private void MoveUpdate()
        {
            if (!enableKeyboardMovement)
            {
                return;
            }

            Vector3.ClampMagnitude(wishDir, maxLength);

            int yNeg = Input.GetKey(KeyCode.Q) ? -1 : 0;
            int yPos = Input.GetKey(KeyCode.E) ? 1 : 0;

            if (Input.GetKey(KeyCode.A))
            {
                cameraInput.x = -1f;
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                cameraInput.x = 0f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                cameraInput.x = 1f;
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                cameraInput.x = 0f;
            }

            if (Input.GetKey(KeyCode.W))
            {
                cameraInput.z = 1f;
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                cameraInput.z = 0f;
            }

            if (Input.GetKey(KeyCode.S))
            {
                cameraInput.z = -1f;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                cameraInput.z = 0f;
            }

            cameraInput.y = yNeg + yPos;

            Transform t = CameraRigManager.Instance.Camera.transform;

            currentSpeed = fastCamera ? fastSpeed : slowSpeed;

            cameraPosition = cameraInput * currentSpeed;

            cameraInput = Vector3.ClampMagnitude(cameraInput, maxLength);

            wishDir = ((t.right * cameraInput.x) + (t.up * cameraInput.y) + (t.forward * cameraInput.z));
            rigidbody.AddForce(wishDir * currentSpeed);
        }
    }
}