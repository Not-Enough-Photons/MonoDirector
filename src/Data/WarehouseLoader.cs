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

namespace NEP.MonoDirector.Data
{
    public static class WarehouseLoader
    {
        internal static List<AudioClip> sounds;

        internal static readonly string companyCode = "NotEnoughPhotons.";
        internal static readonly string modCode = "MonoDirector.";
        internal static readonly string typeCode = "Spawnable.";

        internal static readonly string propMarkerBarcode = CreateFullBarcode("UIPropMarker");
        internal static readonly string infoInterfaceBarcode = CreateFullBarcode("InformationInterface");
        internal static readonly string casterBarcode = CreateFullBarcode("MonoDirectorCasterUI");

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

        internal static List<AssetPoolee> Warmup(string barcode, int size, bool startActive = false)
        {
            List<AssetPoolee> cache = new List<AssetPoolee>();

            for (int i = 0; i < size; i++)
            {
                HelperMethods.SpawnCrate(barcode, Vector3.zero, Quaternion.identity, Vector3.one, false, (obj) => CreateObject(ref cache, obj, startActive));
            }

            return cache;
        }

        private static string CreateFullBarcode(string spawnableName)
        {
            return companyCode + modCode + typeCode + spawnableName;
        }

        private static void CreateObject(ref List<AssetPoolee> cache, GameObject obj, bool startActive = false)
        {
            obj.SetActive(startActive);
            cache.Add(obj.GetComponent<AssetPoolee>());
        }
    }
}
