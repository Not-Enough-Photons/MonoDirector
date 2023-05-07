using System.Reflection;
using System.IO;

using MelonLoader;
using UnityEngine;

using BoneLib.BoneMenu;

using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.Core;
using BoneLib.BoneMenu.Elements;
using NEP.MonoDirector.UI;

namespace NEP.MonoDirector
{
    public static class BuildInfo
    {
        public const string Name = "MonoDirector"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Camera dolly system for Boneworks. Take cool shots and whatnot!"; // Description for the Mod.  (Set as null if none)
        public const string Author = "Not Enough Photons"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "0.0.1"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class Main : MelonMod
    {
        internal static MelonLogger.Instance Logger;

        public static Main instance;

        public static Director director;

        public static FreeCameraRig camera;

        public static FeedbackSFX feedbackSFX;

        public static AssetBundle bundle;

        public override void OnInitializeMelon()
        {
            Logger = new MelonLogger.Instance("MonoDirector", System.ConsoleColor.Magenta);

            instance = this;

            bundle = GetEmbeddedBundle();

            BoneLib.Hooking.OnLevelInitialized += (info) => MonoDirectorInitialize();

            BuildMenu();
        }

        private void MonoDirectorInitialize()
        {
            ResetInstances();
            CreateCamera();
            CreateDirector();
            CreateSFX();
            CreateUI();
        }

        private void ResetInstances()
        {
            Events.FlushActions();
            director = null;
            camera = null;
            feedbackSFX = null;
            PropMarkerManager.CleanUp();
        }

        private void CreateCamera()
        {
            SLZ.Rig.RigManager rigManager = BoneLib.Player.rigManager;
            GameObject gameObject = rigManager.transform.Find("Spectator Camera").gameObject;
            
            // Detach the parent from the rig, so the camera is independent
            gameObject.transform.parent = null;

            camera = gameObject.AddComponent<FreeCameraRig>();
        }

        private void CreateDirector()
        {
            GameObject directorObject = new GameObject("Director");
            director = directorObject.AddComponent<Director>();
            director.SetCamera(camera);
        }

        private void CreateSFX()
        {
            GameObject feedback = new GameObject("Feedback SFX");
            feedbackSFX = feedback.AddComponent<FeedbackSFX>();
        }

        private void CreateUI()
        {
            PropMarkerManager.Initialize();
        }

        private static AssetBundle GetEmbeddedBundle()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            string fileName = "md_resources.pack";

            using (Stream resourceStream = assembly.GetManifestResourceStream("NEP.MonoDirector.Resources." + fileName))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    resourceStream.CopyTo(memoryStream);
                    return AssetBundle.LoadFromMemory(memoryStream.ToArray());
                }
            }
        }

        private void BuildMenu()
        {
            var root = MenuManager.CreateCategory("Not Enough Photons", Color.white);
            var mdMenu = root.CreateCategory("MonoDirector", "#db35b2");
            var playbackCategory = mdMenu.CreateCategory("Playback", Color.white);
            var actorCategory = mdMenu.CreateCategory("Actors", Color.white);
            var settingsCategory = mdMenu.CreateCategory("Settings", Color.white);
            var debug = settingsCategory.CreateCategory("Debug", Color.green);

            BuildPlaybackMenu(playbackCategory);
            BuildActorMenu(actorCategory);
            BuildSettingsMenu(settingsCategory);
            BuildDebugCategory(debug);
        }

        private void BuildPlaybackMenu(MenuCategory category)
        {
            category.CreateFunctionElement("Record", Color.red, () => director.Record());
            category.CreateFunctionElement("Play", Color.green, () => director.Play());
            category.CreateFunctionElement("Pause", Color.yellow, () => director.Pause());
            category.CreateFunctionElement("Stop", Color.red, () => director.Stop());
        }

        private void BuildActorMenu(MenuCategory category)
        {
            category.CreateFunctionElement("Delete Last Actor", Color.red, () => director.RemoveActor(Recorder.instance.LastActor), "Are you sure? This cannot be undone.");
            category.CreateFunctionElement("Remove All Actors", Color.red, () => director.RemoveAllActors(), "Are you sure? This cannot be undone.");
            category.CreateFunctionElement("Clear Scene", Color.red, () => director.ClearScene(), "Are you sure? This cannot be undone.");
        }

        private void BuildSettingsMenu(MenuCategory category)
        {
            var audioCategory = category.CreateCategory("Audio", Color.white);
            var cameraCategory = category.CreateCategory("Camera", Color.white);
            var toolCategory = category.CreateCategory("Tools", Color.white);

            audioCategory.CreateBoolElement("Use Microphone", Color.white, false, (value) => Settings.World.useMicrophone = value);
            audioCategory.CreateBoolElement("Mic Playback", Color.white, false, (value) => Settings.World.micPlayback = value);

            var vfxCategory = cameraCategory.CreateCategory("VFX", Color.white);

            vfxCategory.CreateBoolElement("Lens Distortion", Color.white, true, (value) => Director.instance.Volume.LensDistortion.active = value);
            vfxCategory.CreateBoolElement("Motion Blur", Color.white, true, (value) => Director.instance.Volume.MotionBlur.active = value);
            vfxCategory.CreateBoolElement("Chromatic Abberation", Color.white, true, (value) => Director.instance.Volume.ChromaticAberration.active = value);
            vfxCategory.CreateBoolElement("Vignette", Color.white, true, (value) => Director.instance.Volume.Vignette.active = true);
            vfxCategory.CreateBoolElement("Bloom", Color.white, true, (value) => Director.instance.Volume.Bloom.active = true);
            vfxCategory.CreateBoolElement("MK Glow", Color.white, true, (value) => Director.instance.Volume.MkGlow.active = true);

            toolCategory.CreateFloatElement("Playback Speed", Color.white, 1f, 0.1f, float.NegativeInfinity, float.PositiveInfinity, (value) => Playback.instance.SetPlaybackRate(value));
            toolCategory.CreateFunctionElement("Spectator Head Mode", Color.white, () => Director.instance.Camera.TrackHeadCamera());
        }

        private void BuildDebugCategory(MenuCategory category)
        {
            category.CreateBoolElement("Debug Mode", Color.white, false, (value) => Settings.Debug.debugEnabled = value);
            category.CreateBoolElement("Use Debug Keys", Color.white, false, (value) => Settings.Debug.useKeys = value);
        }
    }
}