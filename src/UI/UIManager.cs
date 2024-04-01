using SLZ.Marrow.Pool;
using System.Collections.Generic;
using UnityEngine;
using BoneLib;

namespace NEP.MonoDirector.UI
{
    public static class UIManager
    {
        internal static readonly string companyCode = "NotEnoughPhotons.";
        internal static readonly string modCode = "MonoDirector.";
        internal static readonly string typeCode = "Spawnable.";

        internal static readonly string propMarkerBarcode = companyCode + modCode + typeCode + "UIPropMarker";
        internal static readonly string infoInterfaceBarcode = companyCode + modCode + typeCode + "InformationInterface";
        internal static readonly string casterBarcode = companyCode + modCode + typeCode + "MonoDirectorCasterUI";

        internal static List<AssetPoolee> Warmup(string barcode, int size, bool startActive = false)
        {
            List<AssetPoolee> cache = new List<AssetPoolee>();

            for (int i = 0; i < size; i++)
            {
                HelperMethods.SpawnCrate(barcode, Vector3.zero, Quaternion.identity, Vector3.one, false, (obj) => CreateObject(ref cache, obj, startActive));
            }

            return cache;
        }

        private static void CreateObject(ref List<AssetPoolee> cache, GameObject obj, bool startActive = false)
        {
            obj.SetActive(startActive);
            cache.Add(obj.GetComponent<AssetPoolee>());
        }
    }
}
