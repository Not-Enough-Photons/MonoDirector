using System;
using System.Collections.Generic;
using System.Linq;

using BoneLib.Nullables;
using NEP.MonoDirector.Actors;
using SLZ.Bonelab;
using SLZ.Marrow.Data;
using SLZ.Marrow.Pool;
using SLZ.Marrow.Warehouse;
using UnityEngine;
using UnityEngine.Animations;

namespace NEP.MonoDirector.UI
{
    public static class PropMarkerManager
    {
        private static Dictionary<Prop, AssetPoolee> markers = new Dictionary<Prop, AssetPoolee>();
        private static List<AssetPoolee> loadedMarkerObjects = new List<AssetPoolee>();
        private static List<AssetPoolee> activeMarkers = new List<AssetPoolee>();

        public static void Initialize()
        {
            Warmup(32);

            Events.OnPropCreated += AddMarkerToProp;
            Events.OnPropRemoved += RemoveMarkerFromProp;
        }

        public static void CleanUp()
        {
            Events.OnPropCreated -= AddMarkerToProp;
            Events.OnPropRemoved -= RemoveMarkerFromProp;

            markers.Clear();
            loadedMarkerObjects.Clear();
            activeMarkers.Clear();
        }

        public static void AddMarkerToProp(Prop prop)
        {
            if (markers.ContainsKey(prop))
            {
                return;
            }

            AssetPoolee asset = loadedMarkerObjects.FirstOrDefault((marker) => !activeMarkers.Contains(marker));

            asset.gameObject.SetActive(true);

            LookAtTarget lookAtTarget = null;

            if(asset.GetComponent<LookAtTarget>() != null)
            {
                lookAtTarget = asset.GetComponent<LookAtTarget>();
            }
            else
            {
                lookAtTarget = asset.gameObject.AddComponent<LookAtTarget>();
            }

            lookAtTarget.targetTransform = BoneLib.Player.playerHead;

            asset.transform.SetParent(prop.transform);
            asset.transform.localPosition = new Vector3(0f, 1.25f + asset.spawnableCrate.ColliderBounds.extents.y, 0f);

            markers.Add(prop, asset);
            activeMarkers.Add(asset);
        }

        public static void RemoveMarkerFromProp(Prop prop)
        {
            if (!markers.ContainsKey(prop))
            {
                return;
            }

            AssetPoolee marker = markers[prop];
            marker.Despawn();

            markers.Remove(prop);
            activeMarkers.Remove(marker);
        }

        private static void Warmup(int size)
        {
            for(int i = 0; i < size; i++)
            {
                string barcode = "NotEnoughPhotons.MonoDirector.Spawnable.UIPropMarker";
                SpawnableCrateReference reference = new SpawnableCrateReference(barcode);

                Spawnable icon = new Spawnable()
                {
                    crateRef = reference
                };

                AssetSpawner.Register(icon);
                NullableMethodExtensions.PoolManager_Spawn(icon, default, default, null, false, null, new Action<GameObject>((obj) => CreateMarker(obj)));
            }
        }

        private static void CreateMarker(GameObject obj)
        {
            obj.SetActive(false);
            loadedMarkerObjects.Add(obj.GetComponent<AssetPoolee>());
        }
    }
}
