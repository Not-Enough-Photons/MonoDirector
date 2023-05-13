﻿using BoneLib.BoneMenu;
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

        internal static Director director = Director.instance;

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
            category.CreateFunctionElement("Record", Color.red, () => director.Record());
            category.CreateFunctionElement("Play", Color.green, () => director.Play());
            category.CreateFunctionElement("Pause", Color.yellow, () => director.Pause());
            category.CreateFunctionElement("Stop", Color.red, () => director.Stop());
        }

        private static void BuildActorMenu(MenuCategory category)
        {
            category.CreateFunctionElement("Delete Last Actor", Color.red, () => director.RemoveActor(Recorder.instance.LastActor), "Are you sure? This cannot be undone.");
            category.CreateFunctionElement("Remove All Actors", Color.red, () => director.RemoveAllActors(), "Are you sure? This cannot be undone.");
            category.CreateFunctionElement("Clear Scene", Color.red, () => director.ClearScene(), "Are you sure? This cannot be undone.");
        }

        private static void BuildSettingsMenu(MenuCategory category)
        {
            var audioCategory = category.CreateCategory("Audio", Color.white);
            var cameraCategory = category.CreateCategory("Camera", Color.white);
            var toolCategory = category.CreateCategory("Tools", Color.white);

            audioCategory.CreateBoolElement("Use Microphone", Color.white, false, (value) => Settings.World.useMicrophone = value);
            audioCategory.CreateBoolElement("Mic Playback", Color.white, false, (value) => Settings.World.micPlayback = value);

            cameraCategory.CreateEnumElement("Camera Mode", Color.white, CameraMode.None, (mode) => CameraRigManager.Instance.SetCameraMode(mode));

            var headModeCategory = cameraCategory.CreateCategory("Head Mode Settings", Color.white);
            var freeCamCategory = cameraCategory.CreateCategory("Free Camera Settings", Color.white);

            BuildHeadModeCategory(headModeCategory);
            BuildFreeModeCategory(freeCamCategory);

            var vfxCategory = cameraCategory.CreateCategory("VFX", Color.white);

            BuildVFXCategory(vfxCategory);

            toolCategory.CreateFloatElement("Playback Speed", Color.white, 1f, 0.1f, float.NegativeInfinity, float.PositiveInfinity, (value) => Playback.instance.SetPlaybackRate(value));
        }

        private static void BuildDebugCategory(MenuCategory category)
        {
            category.CreateBoolElement("Debug Mode", Color.white, false, (value) => Settings.Debug.debugEnabled = value);
            category.CreateBoolElement("Use Debug Keys", Color.white, false, (value) => Settings.Debug.useKeys = value);
        }

        private static void BuildHeadModeCategory(MenuCategory headModeCategory)
        {
            headModeCategory.CreateFloatElement("Interpolation", Color.white, 4f, 1f, 0f, 64f, (value) => CameraRigManager.Instance.SetCameraSmoothness(value));
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
                (value) => CameraRigManager.Instance?.SetMouseSensitivity(value));

            freeModeCategory.CreateFloatElement(
                "Mouse Smoothing",
                Color.white,
                1f,
                0.5f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance?.SetMouseSmoothness(value));

            freeModeCategory.CreateFloatElement(
                "Slow Speed",
                Color.white,
                5f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance?.SetSlowSpeed(value));

            freeModeCategory.CreateFloatElement(
                "Fast Speed",
                Color.white,
                10f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance?.SetFastSpeed(value));

            freeModeCategory.CreateFloatElement(
                "Max Speed",
                Color.white,
                15f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance?.SetMaxSpeed(value));

            freeModeCategory.CreateFloatElement(
                "Friction",
                Color.white,
                5f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance?.SetFriction(value));
        }

        private static void BuildVFXCategory(MenuCategory vfxCategory)
        {
            CameraVolume cameraVolume = CameraRigManager.Instance.CameraVolume;

            vfxCategory.CreateBoolElement("Lens Distortion", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.LensDistortion.active = value);
            //vfxCategory.CreateBoolElement("Motion Blur", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.MotionBlur.active = value);
            vfxCategory.CreateBoolElement("Chromatic Abberation", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.ChromaticAberration.active = value);
            //vfxCategory.CreateBoolElement("Vignette", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.Vignette.active = true);

            //vfxCategory.CreateBoolElement("Bloom", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.Bloom.active = true);
            //vfxCategory.CreateBoolElement("MK Glow", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.MkGlow.active = true);
        }
    }
}