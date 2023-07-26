using System.Reflection;
using System.IO;

using MelonLoader;
using UnityEngine;

using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.UI;

namespace NEP.MonoDirector
{
    public static class BuildInfo
    {
        public const string Name = "MonoDirector"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "A movie/photo making utility for BONELAB!"; // Description for the Mod.  (Set as null if none)
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

        public static FreeCamera camera;

        public static FeedbackSFX feedbackSFX;

        public static AssetBundle bundle;

        public override void OnInitializeMelon()
        {
            Logger = new MelonLogger.Instance("MonoDirector", System.ConsoleColor.Magenta);

            instance = this;

            Directory.CreateDirectory(Constants.dirBase);
            Directory.CreateDirectory(Constants.dirMod);
            Directory.CreateDirectory(Constants.dirSFX);
            Directory.CreateDirectory(Constants.dirImg);

            bundle = GetEmbeddedBundle();

            BoneLib.Hooking.OnLevelInitialized += (info) => MonoDirectorInitialize();

            MDMenu.Initialize();

#if DEBUG
            Logger.Warning("MONODIRECTOR DEBUG BUILD!");
#endif
        }

        private void MonoDirectorInitialize()
        {
            ResetInstances();
            CreateCameraManager();
            CreateDirector();
            CreateSFX();
            CreateUI();

            Data.AvatarPhotoBuilder.Initialize();
        }

        private void ResetInstances()
        {
            Events.FlushActions();
            director = null;
            camera = null;
            feedbackSFX = null;
            PropMarkerManager.CleanUp();
        }

        private void CreateCameraManager()
        {
            new CameraRigManager();
        }

        private void CreateDirector()
        {
            GameObject directorObject = new GameObject("Director");
            director = directorObject.AddComponent<Director>();
            director.SetCamera(camera);
        }

        private void CreateSFX()
        {
            GameObject audioManager = new GameObject("MonoDirector - Audio Manager");
            audioManager.AddComponent<AudioManager>();

            GameObject feedback = new GameObject("Feedback SFX");
            feedbackSFX = feedback.AddComponent<FeedbackSFX>();
        }

        private void CreateUI()
        {
            PropMarkerManager.Initialize();
            InfoInterfaceManager.Initialize();

            UIManager.Warmup(UIManager.casterBarcode, 1, false);
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
    }
}