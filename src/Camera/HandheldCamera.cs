using SLZ.Bonelab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    /// <summary>
    /// Simple camera controller. Can place down points in the world for
    /// the camera rig to follow.
    /// </summary>
    /// 
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class HandheldCamera : CameraRig
    {
        public HandheldCamera(System.IntPtr ptr) : base(ptr) { }

        private Quaternion rot = Quaternion.identity;
        private float delta => Time.deltaTime;

        private bool smoothRotation = true;
        public float smoothDamp = 4f;

        private float xMove = 0f;
        private float yMove = 0f;
        private float zMove = 0f;

        private float xMouseMove = 0f;
        private float yMouseMove = 0f;
        private float zMouseMove = 0f;

        private float xMouse = 0f;
        private float yMouse = 0f;
        private float zMouse = 0f;

        private Rigidbody rigidbody;

        protected override void Awake()
        {
            base.Awake();

            rigidbody = gameObject.AddComponent<Rigidbody>();

            _camera = GetComponent<Camera>();

            GetComponent<SmoothFollower>().enabled = false;

            GameObject test = GameObject.Instantiate(Main.bundle.LoadAsset("md_camera").Cast<GameObject>());
            test.transform.parent = transform;
            test.transform.localPosition = Vector3.forward * -0.1f;
            test.transform.eulerAngles = Vector3.zero;
            test.transform.localScale = new Vector3(0.075f, 0.075f, -0.075f);
        }

        protected override void Update()
        {
            //UpdateRotation(MouseUpdate());
            //UpdateRotationDamp();
        }

        private Quaternion MouseUpdate()
        {
            if (smoothRotation)
            {
                //xMouseMove += x * delta;
                //yMouseMove += y * delta;

                xMouse += xMouseMove;
                yMouse -= yMouseMove;
            }
            else
            {
                //xMouse += x;
                //yMouse -= y;
            }

            return Quaternion.Euler((Vector3.right * yMouse + Vector3.up * xMouse + Vector3.forward * zMouse) * 4f);
        }

        private void UpdateRotationDamp()
        {
            if (xMouseMove > 0f || xMouseMove < 0f)
            {
                xMouseMove = Mathf.Lerp(xMouseMove, 0f, smoothDamp * Time.deltaTime);
            }

            if (yMouseMove > 0f || yMouseMove < 0f)
            {
                yMouseMove = Mathf.Lerp(yMouseMove, 0f, smoothDamp * Time.deltaTime);
            }
        }
    }
}