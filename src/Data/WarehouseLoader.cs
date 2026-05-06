using System.Collections;

using UnityEngine;

using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Pool;
using MelonLoader.Utils;

using Il2CppSLZ.Marrow.Warehouse;

using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.Data
{
    public static class WarehouseLoader
    {
        internal static Dictionary<string, AudioClip> soundTable;
        internal static List<AudioClip> sounds;

        internal static readonly string companyCode = "NEP.";
        internal static readonly string modCode = "MonoDirector.";
        internal static readonly string typeCode = "Spawnable.";

        internal static readonly Barcode propMarkerBarcode = CreateFullBarcode("PropMarker");
        internal static readonly Barcode infoInterfaceBarcode = CreateFullBarcode("InformationInterface");
        internal static readonly Barcode mainMenuBarcode = CreateFullBarcode("MonoDirectorMenu");
        internal static readonly Barcode frameBarcode = CreateFullBarcode("Frame");
        internal static readonly Barcode actorPanelBarcode = CreateFullBarcode("ActorPanel");
        internal static readonly Barcode sceneShelfBarcode = CreateFullBarcode("SceneShelf");

        internal static void LoadSounds()
        {
            sounds = new List<AudioClip>();
            soundTable = new Dictionary<string, AudioClip>();
            string path = Path.Combine(MelonEnvironment.UserDataDirectory, "Not Enough Photons/MonoDirector/SFX");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            IEnumerable<string> files = Directory.EnumerateFiles(path);

            foreach (var file in files)
            {
                var clip = AudioImportLib.API.LoadAudioClip(file, true);
                clip.hideFlags = HideFlags.DontUnloadUnusedAsset;
                sounds.Add(clip);
            }
        }

        internal static void GenerateSpawnablesFromSounds()
        {
            if (sounds == null)
            {
                return;
            }

            if (sounds.Count == 0)
            {
                return;
            }

            Barcode mainBarcode = new Barcode("NEP.MonoDirector");
            if (!AssetWarehouse.Instance.HasPallet(mainBarcode))
            {
                Logging.Error("Pallet doesn't exist in registry.");
                return;
            }

            PalletManifest palletManifest = AssetWarehouse.Instance.palletManifests[mainBarcode];

            if (palletManifest == null)
            {
                Logging.Error("Pallet manifest is null.");
                return;
            }
            
            Pallet pallet = palletManifest.Pallet;

            SpawnableCrate spawnable = null;
            foreach (Crate crate in pallet.Crates)
            {
                if (crate.Barcode == CreateFullBarcode("SoundHolder"))
                {
                    spawnable = crate.Cast<SpawnableCrate>();
                    break;
                }
            }

            if (spawnable == null)
            {
                Logging.Error("Sound holder spawnable is null.");
                return;
            }

            foreach (var sound in sounds)
            {
                SpawnableCrate copyCrate = new SpawnableCrate()
                {
                    Title = $"SFX - {sound.name}",
                    Barcode = new Barcode($"NEP.MonoDirector.Spawnables.SFX{sound.name}"),
                    Description = sound.name,
                    Pallet = spawnable.Pallet,
                    _packedAssets = spawnable.PackedAssets,
                    MainAsset = spawnable.MainAsset,
                    MainGameObject = spawnable.MainGameObject,
                    PreviewMesh = spawnable.PreviewMesh
                };

                copyCrate.name = copyCrate.Title;
                soundTable.Add(copyCrate.Description, sound);
                pallet.Crates.Add(copyCrate);
                AssetWarehouse.Instance.AddCrate(copyCrate);
            }
        }

        internal static IEnumerator SpawnFromBarcode(Barcode barcode, bool active = false)
        {
            SpawnableCrateReference crateRef = new SpawnableCrateReference(barcode);
            Spawnable spawnable = new Spawnable();
            spawnable.crateRef = crateRef;
            
            AssetSpawner.Register(spawnable);

            var task = AssetSpawner.SpawnAsync(
                spawnable,
                Vector3.zero,
                Quaternion.identity,
                new Il2CppSystem.Nullable<Vector3>(Vector3.one),
                null,
                false,
                new Il2CppSystem.Nullable<int>(0)).GetAwaiter();

            while (!task.IsCompleted) yield return null;

            GameObject spawnedObject = task.GetResult().gameObject;
            
            spawnedObject.SetActive(active);
            
            yield return null;
        }

        private static Barcode CreateFullBarcode(string spawnableName)
        {
            return new Barcode(companyCode + modCode + typeCode + spawnableName);
        }
    }
}
