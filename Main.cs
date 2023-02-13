using System.Reflection;
using System.IO;

using MelonLoader;
using UnityEngine;

using BoneLib;
using BoneLib.BoneMenu;

using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.State;
using NEP.MonoDirector.UI;
using NEP.MonoDirector.UI.Interface;

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

        public override void OnUpdate()
        {

        }

        private void MonoDirectorInitialize()
        {
            Events.FlushActions();
            director = null;
            feedbackSFX = null;

            //UIManager.Construct();
            var test = GameObject.Instantiate(bundle.LoadAsset("md_main_menu")).Cast<GameObject>();
            test.AddComponent<RootPanel>();

            SLZ.Rig.RigManager rigManager = BoneLib.Player.rigManager;
            GameObject gameObject = rigManager.transform.Find("Spectator Camera").gameObject;
            gameObject.AddComponent<FreeCameraRig>();

            GameObject directorTrackObject = new GameObject("Director");
            director = directorTrackObject.AddComponent<Director>();
            director.SetCamera(gameObject.GetComponent<FreeCameraRig>());

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

            //playbackCategory.CreateEnumElement<CaptureState>("Capture Type", Color.white, (type) => director.captureState = type);
            playbackCategory.CreateFunctionElement("Record", Color.red, () => director.Record());
            playbackCategory.CreateFunctionElement("Play", Color.green, () => director.Play());
            playbackCategory.CreateFunctionElement("Pause", Color.yellow, () => director.Pause());
            playbackCategory.CreateFunctionElement("Stop", Color.red, () => director.Stop());

            actorCategory.CreateFunctionElement("Remove All Actors", Color.red, () => director.RemoveAllActors(), "Are you sure? This cannot be undone.");
            actorCategory.CreateFunctionElement("Clear Scene", Color.red, () => director.ClearScene(), "Are you sure? This cannot be undone.");

            settingsCategory.CreateBoolElement("Spawn Gun Sets Props", Color.white, false, (value) => Settings.World.spawnGunProps = value);
        }
    }
}