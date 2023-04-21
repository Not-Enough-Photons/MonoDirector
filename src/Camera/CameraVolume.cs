using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using MK.Glow.URP;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class CameraVolume : MonoBehaviour
    {
        public CameraVolume(System.IntPtr ptr) : base(ptr) { }

        public Volume RenderingVolume { get => renderingVolume; }

        public LensDistortion LensDistortion { get => vfxLensDistortion; }
        public ChromaticAberration ChromaticAberration { get => vfxChromaticAbberration; }
        public MKGlow MKGlow { get => vfxMKGlow; }
        public MotionBlur MotionBlur { get => vfxMotionBlur; }
        public Vignette Vignette { get => vfxVignette; }
        public Bloom Bloom { get => vfxBloom; }

        private Volume renderingVolume;

        private LensDistortion vfxLensDistortion;
        private ChromaticAberration vfxChromaticAbberration;
        private MKGlow vfxMKGlow;

        private MotionBlur vfxMotionBlur;
        private Vignette vfxVignette;
        private Bloom vfxBloom;

        private void Start()
        {
            renderingVolume = GetComponent<Volume>();

            vfxLensDistortion = renderingVolume.profile.components[0].Cast<LensDistortion>();
            vfxChromaticAbberration = renderingVolume.profile.components[1].Cast<ChromaticAberration>();
            vfxMKGlow = renderingVolume.profile.components[4].Cast<MKGlow>();

            vfxMotionBlur = renderingVolume.profile.Add<MotionBlur>(true);
            vfxVignette = renderingVolume.profile.Add<Vignette>(true);
            vfxBloom = renderingVolume.profile.Add<Bloom>(true);
        }

        public void EnableAll(bool enabled)
        {
            foreach(VolumeComponent component in renderingVolume.profile.components)
            {
                component.SetAllOverridesTo(enabled);
            }
        }
    }
}
