using BoneLib.BoneMenu;
using BoneLib.BoneMenu.Elements;

using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.State;

using UnityEngine;

using SLZ.Rig;
using BoneLib;

namespace NEP.MonoDirector.UI
{
    internal static class MDBoneMenu
    {
        internal static MenuCategory rootCategory;

        internal static MenuCategory mdCategory;

        internal static MenuCategory playbackCategory;
        internal static MenuCategory actorCategory;
        internal static MenuCategory settingsCategory;

        internal static void Initialize()
        {
            rootCategory = MenuManager.CreateCategory("Not Enough Photons", Color.white);

            mdCategory = rootCategory.CreateCategory("Mono<color=red>Director</color>", Color.white);

            mdCategory.CreateFunctionElement(
                "Record", 
                Color.red, 
                () => Director.instance.Record()
            );
            
            mdCategory.CreateFunctionElement(
                "Play", 
                Color.green, 
                () => Director.instance.Play()
            );
            
            mdCategory.CreateFunctionElement(
                "Stop", 
                Color.red, 
                () => Director.instance.Stop()
            );

            mdCategory.CreateFunctionElement("Actors", Color.white, () => { MDMenu.instance.gameObject.SetActive(true); });
            settingsCategory = mdCategory.CreateCategory("Settings", Color.white);

            // BuildActorMenu(actorCategory);
            BuildSettingsMenu(settingsCategory);
        }

        private static void BuildActorMenu(MenuCategory category)
        {
            category.CreateFunctionElement(
                "Show Caster Menu", 
                Color.white,
                () => 
                {
                    MDMenu.instance.gameObject.SetActive(true);
                }
            );
        }

        private static void BuildSettingsMenu(MenuCategory category)
        {
            MenuCategory audioCategory = category.CreateCategory("Audio", Color.white);
            MenuCategory cameraCategory = category.CreateCategory("Camera", Color.white);
            MenuCategory toolCategory = category.CreateCategory("Tools", Color.white);
            MenuCategory uiCategory = category.CreateCategory("UI", Color.white);

            MenuCategory headModeCategory = cameraCategory.CreateCategory("Head Mode Settings", Color.white);
            MenuCategory freeCamCategory = cameraCategory.CreateCategory("Free Camera Settings", Color.white);
            MenuCategory vfxCategory = cameraCategory.CreateCategory("VFX", Color.white);
            
            #if DEBUG
            MenuCategory debugCategory = category.CreateCategory("DEBUG", Color.red);
            BuildDebugCategory(debugCategory);
            #endif

            audioCategory.CreateBoolElement(
                "Use Microphone", 
                Color.white, 
                false,
                value => Settings.World.useMicrophone = value
            );
            
            audioCategory.CreateBoolElement(
                "Mic Playback", 
                Color.white, 
                false,
                value => Settings.World.micPlayback = value
            );

            cameraCategory.CreateEnumElement(
                "Camera Mode", 
                Color.white, 
                CameraMode.None,
                (mode) => CameraRigManager.Instance.CameraMode = mode
            );

            cameraCategory.CreateBoolElement(
                "Kinematic On Release", 
                Color.white, 
                false,
                (value) => Settings.Camera.handheldKinematicOnRelease = value
            );

            BuildHeadModeCategory(headModeCategory);
            BuildFreeModeCategory(freeCamCategory);

            BuildVFXCategory(vfxCategory);

            BuildToolCategory(toolCategory);

            BuildUIMenu(uiCategory);
        }

        private static void BuildToolCategory(MenuCategory category)
        {
            category.CreateFloatElement(
                "Playback Speed",
                Color.white,
                1f,
                0.1f,
                float.NegativeInfinity,
                float.PositiveInfinity,
                value => Playback.Instance.PlaybackRate = value
            );

            category.CreateIntElement(
                "Delay",
                Color.white,
                5,
                1,
                0,
                30,
                value => Settings.World.delay = value
            );

            category.CreateIntElement(
                "FPS",
                Color.white,
                60,
                5,
                5,
                160,
                value => Settings.World.fps = value
            );

            category.CreateBoolElement(
                "Ignore Slomo",
                Color.white,
                false,
                value => Settings.World.ignoreSlomo = value
            );

            category.CreateBoolElement(
                "Temporal Scaling",
                Color.white,
                true,
                value => Settings.World.temporalScaling = value
            );
        }

        private static void BuildUIMenu(MenuCategory category)
        {
            category.CreateBoolElement(
                "Show UI",
                Color.white,
                false,
                value => InformationInterface.Instance.ShowUI = value
            );

            category.CreateBoolElement(
                "Show Timecode",
                Color.white,
                false,
                value => InformationInterface.Instance.ShowTimecode = value
            );

            category.CreateBoolElement(
                "Show Play Mode",
                Color.white,
                false,
                value => InformationInterface.Instance.ShowPlaymode = value
            );

            category.CreateBoolElement(
                "Show Icons",
                Color.white,
                false,
                value => InformationInterface.Instance.ShowIcons = value
            );
        }

#if DEBUG
        private static void BuildDebugCategory(MenuCategory category)
        {
            category.CreateFunctionElement(
                "Duplicate Player",
                Color.white,
                () =>
                {
                    RigManager rigManager = BoneLib.Player.rigManager;
                    rigManager.AvatarCrate.Crate.Spawn(rigManager.ControllerRig.m_head.position, Quaternion.identity);
                }
            );
            
            category.CreateBoolElement(
                "Debug Mode", 
                Color.white, 
                false,
                value => Settings.Debug.debugEnabled = value
            );
            
            category.CreateBoolElement(
                "Use Debug Keys", 
                Color.white, 
                false, 
                value => Settings.Debug.useKeys = value
            );
        }
#endif
        
        private static void BuildHeadModeCategory(MenuCategory headModeCategory)
        {
            headModeCategory.CreateFloatElement(
                "Interpolation", 
                Color.white, 
                4f, 
                1f, 
                0f, 
                64f,
                value => CameraRigManager.Instance.CameraSmoothness = value
            );
            
            headModeCategory.CreateEnumElement(
                "Position", 
                Color.white, 
                BodyPart.Head,
                bone => CameraRigManager.Instance.FollowCamera.SetFollowBone(bone)
            );
        }

        private static void BuildFreeModeCategory(MenuCategory freeModeCategory)
        {
            freeModeCategory.CreateFloatElement(
                "Mouse Sens.",
                Color.white,
                1f,
                0.5f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.MouseSensitivity = value);

            freeModeCategory.CreateFloatElement(
                "Mouse Smoothing",
                Color.white,
                1f,
                0.5f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.MouseSmoothness = value
            );

            freeModeCategory.CreateFloatElement(
                "Slow Speed",
                Color.white,
                5f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.SlowSpeed = value
            );

            freeModeCategory.CreateFloatElement(
                "Fast Speed",
                Color.white,
                10f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.FastSpeed = value
            );

            freeModeCategory.CreateFloatElement(
                "Max Speed",
                Color.white,
                15f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.MaxSpeed = value
            );

            freeModeCategory.CreateFloatElement(
                "Friction",
                Color.white,
                5f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.Friction = value
            );
        }

        private static void BuildVFXCategory(MenuCategory vfxCategory)
        {
            vfxCategory.CreateBoolElement(
                "Lens Distortion", 
                Color.white, 
                true,
                value => CameraRigManager.Instance.EnableLensDistortion(value)
            );
            
            //vfxCategory.CreateBoolElement("Motion Blur", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.MotionBlur.active = value);
            
            vfxCategory.CreateBoolElement(
                "Chromatic Abberation", 
                Color.white, 
                true,
                value => CameraRigManager.Instance.EnableChromaticAbberation(value)
            );
            
            //vfxCategory.CreateBoolElement("Vignette", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.Vignette.active = true);

            //vfxCategory.CreateBoolElement("Bloom", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.Bloom.active = true);
            //vfxCategory.CreateBoolElement("MK Glow", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.MkGlow.active = true);
        }
    }
}