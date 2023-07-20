using BoneLib.BoneMenu;
using BoneLib.BoneMenu.Elements;
using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.State;
using UnityEngine;

namespace NEP.MonoDirector.UI
{
    internal static class MDMenu
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

            playbackCategory = mdCategory.CreateCategory("Playback", Color.white);
            actorCategory = mdCategory.CreateCategory("Actors", Color.white);
            settingsCategory = mdCategory.CreateCategory("Settings", Color.white);

            BuildPlaybackMenu(playbackCategory);
            BuildActorMenu(actorCategory);
            BuildSettingsMenu(settingsCategory);
        }

        private static void BuildPlaybackMenu(MenuCategory category)
        {
            category.CreateFunctionElement("Record", Color.red, () => Director.instance.Record());
            category.CreateFunctionElement("Play", Color.green, () => Director.instance.Play());
            category.CreateFunctionElement("Pause", Color.yellow, () => Director.instance.Pause());
            category.CreateFunctionElement("Stop", Color.red, () => Director.instance.Stop());
        }

        private static void BuildActorMenu(MenuCategory category)
        {
            category.CreateFunctionElement("Show Caster Menu", Color.white, () => MenuUI.Instance.gameObject.SetActive(true));
            category.CreateFunctionElement("Delete Last Actor", Color.red, () => Director.instance.RemoveActor(Recorder.instance.LastActor), "Are you sure? This cannot be undone.");
            category.CreateFunctionElement("Remove All Actors", Color.red, () => Director.instance.RemoveAllActors(), "Are you sure? This cannot be undone.");
            category.CreateFunctionElement("Clear Scene", Color.red, () => Director.instance.ClearScene(), "Are you sure? This cannot be undone.");
        }

        private static void BuildSettingsMenu(MenuCategory category)
        {
            var audioCategory = category.CreateCategory("Audio", Color.white);
            var cameraCategory = category.CreateCategory("Camera", Color.white);
            var toolCategory = category.CreateCategory("Tools", Color.white);
            var uiCategory = category.CreateCategory("UI", Color.white);

            audioCategory.CreateBoolElement("Use Microphone", Color.white, false, (value) => Settings.World.useMicrophone = value);
            audioCategory.CreateBoolElement("Mic Playback", Color.white, false, (value) => Settings.World.micPlayback = value);

            cameraCategory.CreateEnumElement("Camera Mode", Color.white, CameraMode.None, (mode) => CameraRigManager.Instance.CameraMode = mode);

            cameraCategory.CreateBoolElement("Lock X Rotation", Color.white, false, (value) => Settings.Camera.handheldLockXAxis = value);
            cameraCategory.CreateBoolElement("Lock Y Rotation", Color.white, false, (value) => Settings.Camera.handheldLockYAxis = value);
            cameraCategory.CreateBoolElement("Lock Z Rotation", Color.white, false, (value) => Settings.Camera.handheldLockZAxis = value);

            cameraCategory.CreateBoolElement("Kinematic On Release", Color.white, false, (value) => Settings.Camera.handheldKinematicOnRelease = value);

            var headModeCategory = cameraCategory.CreateCategory("Head Mode Settings", Color.white);
            var freeCamCategory = cameraCategory.CreateCategory("Free Camera Settings", Color.white);

            BuildHeadModeCategory(headModeCategory);
            BuildFreeModeCategory(freeCamCategory);

            var vfxCategory = cameraCategory.CreateCategory("VFX", Color.white);

            BuildVFXCategory(vfxCategory);

            toolCategory.CreateFloatElement("Playback Speed", Color.white, 1f, 0.1f, float.NegativeInfinity, float.PositiveInfinity, (value) => Playback.instance.SetPlaybackRate(value));
            toolCategory.CreateIntElement("Delay", Color.white, 5, 1, 0, 30, (value) => Settings.World.delay = value);
            toolCategory.CreateIntElement("FPS", Color.white, 90, 5, 5, 144, (value) => Settings.World.fps = value);

            BuildUIMenu(uiCategory);
        }

        private static void BuildUIMenu(MenuCategory category)
        {
            category.CreateBoolElement("Show UI", Color.white, false, (value) => InformationInterface.Instance.ShowUI = value);
            category.CreateBoolElement("Show Timecode", Color.white, false, (value) => InformationInterface.Instance.ShowTimecode = value);
            category.CreateBoolElement("Show Play Mode", Color.white, false, (value) => InformationInterface.Instance.ShowPlaymode = value);
            category.CreateBoolElement("Show Icons", Color.white, false, (value) => InformationInterface.Instance.ShowIcons = value);
        }

        private static void BuildDebugCategory(MenuCategory category)
        {
            category.CreateBoolElement("Debug Mode", Color.white, false, (value) => Settings.Debug.debugEnabled = value);
            category.CreateBoolElement("Use Debug Keys", Color.white, false, (value) => Settings.Debug.useKeys = value);
        }

        private static void BuildHeadModeCategory(MenuCategory headModeCategory)
        {
            headModeCategory.CreateFloatElement("Interpolation", Color.white, 4f, 1f, 0f, 64f, (value) => CameraRigManager.Instance.CameraSmoothness = value);
            headModeCategory.CreateEnumElement("Position", Color.white, BodyPart.Head, (bone) => CameraRigManager.Instance.FollowCamera.SetFollowBone(bone));
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
                (value) => CameraRigManager.Instance.MouseSmoothness = value);

            freeModeCategory.CreateFloatElement(
                "Slow Speed",
                Color.white,
                5f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.SlowSpeed = value);

            freeModeCategory.CreateFloatElement(
                "Fast Speed",
                Color.white,
                10f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.FastSpeed = value);

            freeModeCategory.CreateFloatElement(
                "Max Speed",
                Color.white,
                15f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.MaxSpeed = value);

            freeModeCategory.CreateFloatElement(
                "Friction",
                Color.white,
                5f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.Friction = value);
        }

        private static void BuildVFXCategory(MenuCategory vfxCategory)
        {
            vfxCategory.CreateBoolElement("Lens Distortion", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.LensDistortion.active = value);
            //vfxCategory.CreateBoolElement("Motion Blur", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.MotionBlur.active = value);
            vfxCategory.CreateBoolElement("Chromatic Abberation", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.ChromaticAberration.active = value);
            //vfxCategory.CreateBoolElement("Vignette", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.Vignette.active = true);

            //vfxCategory.CreateBoolElement("Bloom", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.Bloom.active = true);
            //vfxCategory.CreateBoolElement("MK Glow", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.MkGlow.active = true);
        }
    }
}
