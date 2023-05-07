using System;
using System.Collections.Generic;
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

        public static void Initialize()
        {
            Events.OnPropCreated += AddMarkerToProp;
            Events.OnPropRemoved += RemoveMarkerFromProp;
        }

        public static void CleanUp()
        {
            Events.OnPropCreated -= AddMarkerToProp;
            Events.OnPropRemoved -= RemoveMarkerFromProp;

            markers.Clear();
        }

        public static void AddMarkerToProp(Prop prop)
        {
            if (markers.ContainsKey(prop))
            {
                return;
            }

            AssetPoolee asset = CreateMarker();
            var lookAtTarget = asset.gameObject.AddComponent<LookAtTarget>();

            lookAtTarget.targetTransform = BoneLib.Player.playerHead;

            asset.transform.SetParent(prop.transform);
            asset.transform.localPosition = new Vector3(0f, 1f + asset.spawnableCrate.ColliderBounds.extents.y, 0f);

            markers.Add(prop, asset);
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
        }

        private static AssetPoolee CreateMarker()
        {
            string barcode = "NotEnoughPhotons.MonoDirector.Spawnable.UIPropMarker";
            SpawnableCrateReference reference = new SpawnableCrateReference(barcode);

            Spawnable icon = new Spawnable()
            {
                crateRef = reference
            };

            GameObject obj = null;

            Action<GameObject> del = new Action<GameObject>((gameObject) => obj = gameObject);

            AssetSpawner.Register(icon);
            NullableMethodExtensions.PoolManager_Spawn(icon, Vector3.zero, Quaternion.identity, Vector3.one, false, null, del);

            return obj.GetComponent<AssetPoolee>();
        }
    }
}
