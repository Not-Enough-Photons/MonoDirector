using System.Reflection;
using System.IO;

using MelonLoader;
using UnityEngine;

using BoneLib.BoneMenu;

using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.Core;
using BoneLib.BoneMenu.Elements;

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
            CreateDirector();
            CreateCamera();
            CreateSFX();
        }

        private void ResetInstances()
        {
            Events.FlushActions();
            director = null;
            feedbackSFX = null;
        }

        private void CreateCamera()
        {
            SLZ.Rig.RigManager rigManager = BoneLib.Player.rigManager;
            GameObject gameObject = rigManager.transform.Find("Spectator Camera").gameObject;
            camera = gameObject.AddComponent<FreeCameraRig>();
            gameObject.AddComponent<SplinePlacer>();
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
            category.CreateFunctionElement("Retake Shot", Color.magenta, () => director.Retake());
            category.CreateFunctionElement("Stop", Color.red, () => director.Stop());
        }

        private void BuildActorMenu(MenuCategory category)
        {
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

            toolCategory.CreateBoolElement("Spawn Gun Sets Props", Color.white, false, (value) => Settings.World.spawnGunProps = value);
            toolCategory.CreateBoolElement("Spawn Gun Sets NPCs", Color.white, false, (value) => Settings.World.spawnGunNPCs = value);
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