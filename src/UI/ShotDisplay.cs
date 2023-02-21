using NEP.MonoDirector.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ShotDisplay : MonoBehaviour
    {
        public ShotDisplay(IntPtr ptr) : base(ptr) { }

        public Camera Camera { get => camera; }

        private Camera camera;
        private RenderTexture outputTexture;
        private RawImage outputImage;

        private void Awake()
        {
            outputImage = GetComponentInChildren<RawImage>();
        }

        private void OnEnable()
        {
            SetCamera(Director.instance.Camera.cam);
        }

        private void OnDisable()
        {
            SetCamera(null);
        }

        private void Update()
        {
            Display();
        }

        public void SetCamera(Camera camera)
        {
            this.camera = camera;
        }

        public void Display()
        {
            if(camera == null)
            {
                return;
            }

            Graphics.Blit(camera.activeTexture, outputTexture);
            outputImage.texture = outputTexture;
        }
    }
}
