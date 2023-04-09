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

        private Rigidbody rigidbody;

        protected override void Awake()
        {
            base.Awake();

            rigidbody = gameObject.AddComponent<Rigidbody>();

            _camera = GetComponent<UnityEngine.Camera>();

            GetComponent<SmoothFollower>().enabled = false;

            GameObject test = GameObject.Instantiate(Main.bundle.LoadAsset("md_camera").Cast<GameObject>());
            test.transform.parent = transform;
            test.transform.localPosition = Vector3.forward * -0.1f;
            test.transform.eulerAngles = Vector3.zero;
            test.transform.localScale = new Vector3(0.075f, 0.075f, -0.075f);
        }
    }
}