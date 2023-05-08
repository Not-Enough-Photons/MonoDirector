using System;
using System.Collections.Generic;

using BoneLib.Nullables;

using NEP.MonoDirector.Data;
using NEP.MonoDirector.Core;

using SLZ.Marrow.Data;
using SLZ.Marrow.Pool;
using SLZ.Marrow.Warehouse;
using SLZ.Props.Weapons;

using UnityEngine;
using SLZ.Interaction;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class GunProp : Prop
    {
        public GunProp(IntPtr ptr) : base(ptr) { }

        public Gun Gun { get => gun; }

        private Gun gun;

        protected override void Awake()
        {
            base.Awake();

            propFrames = new List<ObjectFrame>();
            actionFrames = new List<ActionFrame>();
        }

        public override void OnSceneBegin()
        {
            base.OnSceneBegin();

            foreach(ActionFrame actionFrame in actionFrames)
            {
                actionFrame.Reset();
            }
        }

        public void GunFakeFire()
        {
            MuzzleFlash();
            EjectCasing();
            gun.gunSFX.GunShot();
        }

        public void SetGun(Gun gun)
        {
            this.gun = gun;
        }

        private void MuzzleFlash()
        {
            string muzzleFlashBarcode = "c1534c5a-93e8-405b-89e2-e39c466c6172";

            SpawnableCrateReference reference = new SpawnableCrateReference(muzzleFlashBarcode);

            Spawnable muzzleFlash = new Spawnable()
            {
                crateRef = reference
            };

            AssetSpawner.Register(muzzleFlash);
            NullableMethodExtensions.PoolManager_Spawn(
                muzzleFlash,
                gun.firePointTransform.position,
                gun.firePointTransform.rotation,
                Vector3.one,
                false,
                null,
                null,
                null);
        }

        private void EjectCasing()
        {
            SpawnableCrateReference reference = gun.defaultCartridge.cartridgeCaseSpawnable.crateRef;
            Spawnable casing = new Spawnable()
            {
                crateRef = reference
            };

            AssetSpawner.Register(casing);
            NullableMethodExtensions.PoolManager_Spawn(
                casing,
                gun.shellSpawnTransform.position,
                gun.shellOrientationTransform.rotation,
                Vector3.one,
                false,
                null,
                null,
                null);
        }
    }
}
