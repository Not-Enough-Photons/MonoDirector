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
            Spawnable muzzleFlashSpawnable = new Spawnable()
            {
                crateRef = gun.muzzleFlareSpawnable.crateRef
            };
            AssetSpawner.Register(muzzleFlashSpawnable);
            NullableMethodExtensions.PoolManager_Spawn(
                muzzleFlashSpawnable,
                gun.firePointTransform.position,
                gun.firePointTransform.rotation,
                null,
                false,
                null,
                null,
                null);
        }

        private void EjectCasing()
        {
            Spawnable cartridgeSpawnable = new Spawnable()
            {
                crateRef = gun.defaultCartridge.cartridgeCaseSpawnable.crateRef
            };
            AssetSpawner.Register(cartridgeSpawnable);
            NullableMethodExtensions.PoolManager_Spawn(
                cartridgeSpawnable,
                gun.shellSpawnTransform.position,
                gun.shellOrientationTransform.rotation,
                null,
                false,
                null,
                null,
                null);
        }
    }
}
