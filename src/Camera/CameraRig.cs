using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class CameraRig : MonoBehaviour
    {
        public CameraRig(System.IntPtr ptr) : base(ptr) { }

        protected float _time = 0f;

        protected Camera _camera;
        public Camera cam
        {
            get
            {
                return _camera;
            }
        }

        protected virtual void Awake()
        {
            _camera = gameObject.GetComponent<Camera>();
        }

        protected virtual void OnDisable() { }

        protected virtual void Update() { }

        protected virtual void UpdateFOV(float fov)
{
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, fov, Time.deltaTime);
        }
    }

}