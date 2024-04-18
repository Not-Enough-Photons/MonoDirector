using SLZ.Marrow.Pool;
using System.Collections.Generic;
using UnityEngine;
using BoneLib;
using System.Threading.Tasks;
using System.IO;
using MelonLoader;
using System;
using SLZ.Marrow.Data;
using SLZ.Marrow.Warehouse;
using UnhollowerBaseLib;

namespace NEP.MonoDirector.Data
{
    public static class WarehouseLoader
    {
        internal static List<AudioClip> sounds;

        internal static readonly string companyCode = "NotEnoughPhotons.";
        internal static readonly string modCode = "MonoDirector.";
        internal static readonly string typeCode = "Spawnable.";

        internal static readonly string propMarkerBarcode = CreateFullBarcode("PropMarker");
        internal static readonly string infoInterfaceBarcode = CreateFullBarcode("InformationInterface");
        internal static readonly string mainMenuBarcode = CreateFullBarcode("MonoDirectorMenu");

        internal static List<AudioClip> GetSounds()
        {
            List<AudioClip> sounds = new List<AudioClip>();
            string path = Path.Combine(MelonUtils.UserDataDirectory, "Not Enough Photons/MonoDirector/SFX/Sounds");
            IEnumerable<string> files = Directory.EnumerateFiles(path);

            foreach(var file in files)
            {
                var clip = AudioImportLib.API.LoadAudioClip(file, true);
                sounds.Add(clip);
            }

            return sounds;
        }

        internal static void GenerateSpawnablesFromSounds(AudioClip[] sounds)
        {
            Barcode mainBarcode = (Barcode)"NotEnoughPhotons.MonoDirector";
            if (!AssetWarehouse.Instance.HasPallet(mainBarcode))
            {
                Main.Logger.Error("Pallet doesn't exist in registry.");
                return;
            }
            
            var mainPallet = AssetWarehouse.Instance.InventoryRegistry[mainBarcode] as Pallet;

            if (mainPallet == null)
            {
                Main.Logger.Error("Pallet doesn't exist.");
                return;
            }

            SpawnableCrate spawnable = null;
            foreach(Crate crate in mainPallet.Crates)
            {
                Main.Logger.Msg(crate.Barcode);
                if (crate.Barcode == (Barcode)CreateFullBarcode("SoundHolder"))
                {
                    spawnable = (SpawnableCrate)crate;
                    break;
                }
            }

            if (spawnable == null)
            {
                return;
            }
        }

        internal static GameObject SpawnFromBarcode(string barcode, bool active = false)
        {
            GameObject spawnedObject = null;
            HelperMethods.SpawnCrate(barcode, Vector3.zero, Quaternion.identity, Vector3.one, false, (obj) => 
            {
                spawnedObject = obj;
                spawnedObject.SetActive(active);
            });

            return spawnedObject;
        }

        internal static List<GameObject> SpawnFromBarcode(string barcode, int amount, bool active = false)
        {
            List<GameObject> spawnedObjects = new List<GameObject>();

            for (int i = 0; i < amount; i++)
            {
                var obj = SpawnFromBarcode(barcode, active);
                obj.SetActive(active);
                spawnedObjects.Add(obj);
            }

            return spawnedObjects;
        }

        private static string CreateFullBarcode(string spawnableName)
        {
            return companyCode + modCode + typeCode + spawnableName;
        }
    }
}
