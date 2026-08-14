using UnityEngine;

using BoneLib;

using MelonLoader;
using Il2CppSLZ.Marrow.Warehouse;
using BoneLib.Notifications;
using NEP.MonoDirector.Content;
using NEP.MonoDirector.Compatibility;
using NEP.MonoDirector.UI;
using NEP.MonoDirector.Visuals;

namespace NEP.MonoDirector.Core;

public static class Engine
{
    internal static GameObject MainContainerObject { get; private set; }

    internal static bool AudioImportInstalled { get => m_audioImportInstalled; }

    private static bool m_audioImportInstalled = false;

    internal static void Initialize()
    {
        ModCompatibility.Scan();
        Logging.Initialize();
        BundleLoader.Initialize();
        MDBoneMenu.Initialize();

        MelonCoroutines.Start(AssetDownloader.Start());

        // Directory.CreateDirectory(Constants.dirBase);
        // Directory.CreateDirectory(Constants.dirMod);
        // Directory.CreateDirectory(Constants.dirSFX);
        // Directory.CreateDirectory(Constants.dirImg);

        Hooking.OnLevelLoaded += OnLevelLoaded;
        Hooking.OnWarehouseReady += OnWarehouseReady;

        CheckAudioImport();
    }
    
    internal static void Shutdown()
    {
        
    }

    internal static void Update()
    {
        Director.Update();
        VisualManager.Update();
    }
    
    internal static void OnWarehouseReady()
    {
        AssetWarehouse.Instance.OnPalletAdded += new Action<Barcode>(OnPalletAdded);

        if (!m_audioImportInstalled)
        {
            Logging.Warn("AudioImportLib is not installed!");
            return;
        }

        if (AssetDownloader.CheckInstall())
        {
            WarehouseLoader.LoadSounds();
            WarehouseLoader.GenerateSpawnablesFromSounds();
        }
    }

    internal static void OnPalletAdded(Barcode barcode)
    {
        if (barcode.ID != "NEP.MonoDirector")
        {
            return;
        }

        if (!m_audioImportInstalled)
        {
            Notification notification = new()
            {
                Title = "Missing AudioImportLib",
                Message = "You do not have AudioImportLib installed! Custom sounds will not work.",
                Type = NotificationType.Warning,
                PopupLength = 5f
            };

            Notifier.Send(notification);

            return;
        }

        WarehouseLoader.LoadSounds();
        WarehouseLoader.GenerateSpawnablesFromSounds();
    }

    internal static void OnLevelLoaded(LevelInfo info)
    {
        if (!m_audioImportInstalled)
        {
            Notification notification = new()
            {
                Title = "Missing AudioImportLib",
                Message = "You do not have AudioImportLib installed! Custom sounds will not work.",
                Type = NotificationType.Warning,
                PopupLength = 5f
            };

            Notifier.Send(notification);
        }

        if (!AssetDownloader.CheckInstall())
        {
            Notification notification = new()
            {
                Title = "Missing Content Pallet",
                Message = "You do not have the MonoDirector content pallet installed! Subscribe to it on mod.io, then install it in game!",
                Type = NotificationType.Warning,
                PopupLength = 5f
            };

            Notifier.Send(notification);
        }
        else if(AssetDownloader.NeedsNewVersion())
        {
            Notification notification = new()
            {
                Title = "Content Update Available",
                Message = "A new update for the MonoDirector pallet is available! Download it from Void G114!",
                Type = NotificationType.Warning,
                PopupLength = 5f
            };

            Notifier.Send(notification);
        }

        MainContainerObject = new GameObject("[MonoDirector]");

        Director.Initialize();
        VisualManager.Initialize();
        
        // CreateUI();
    }

    internal static void CreateUI()
    {
        //MelonCoroutines.Start(WarehouseLoader.SpawnFromBarcode(WarehouseLoader.actorPanelBarcode, true));
        //MelonCoroutines.Start(WarehouseLoader.SpawnFromBarcode(WarehouseLoader.mainMenuBarcode));
        //MelonCoroutines.Start(WarehouseLoader.SpawnFromBarcode(WarehouseLoader.sceneShelfBarcode));
        //MelonCoroutines.Start(WarehouseLoader.SpawnFromBarcode(WarehouseLoader.mainMenuBarcode));
    }

    private static void CheckAudioImport()
    {
        if (MelonBase.FindMelon("AudioImportLib", "trev & zCubed") != null)
            m_audioImportInstalled = true;
    }

    internal static void AnnounceError()
    {
        var mixer = BoneLib.Audio.InHead;
        BoneLib.Audio.Play2DOneShot(BundleLoader.ErrorClip, mixer, 0.1f);
    }
}
