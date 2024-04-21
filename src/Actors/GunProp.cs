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
using SLZ.Props;
using static SLZ.Props.Weapons.Gun;

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
            gun.cartridgeState = CartridgeStates.SPENT;
            gun.UpdateArt();
            
            MuzzleFlash();
            //EjectCasing();
            gun.gunSFX.GunShot();
        }

        public void SetGun(Gun gun)
        {
            this.gun = gun;
            SlideVirtualController slideVirtualController = gun.slideVirtualController;
            if (slideVirtualController != null)
            {
                gun.slideVirtualController.OnSlideGrabbed = gun.slideVirtualController.OnSlideGrabbed + new System.Action(() => RecordSlideGrabbed());
                gun.slideVirtualController.OnSlideReleased = gun.slideVirtualController.OnSlideReleased + new System.Action(() => RecordSlideReleased());
                gun.slideVirtualController.OnSlidePulled = gun.slideVirtualController.OnSlidePulled + new System.Action(() => RecordSlidePulled());
                gun.slideVirtualController.OnSlideUpdate = gun.slideVirtualController.OnSlideUpdate + new System.Action<float>((float perc) => RecordSlideUpdate(perc));
                gun.slideVirtualController.OnSlideReturned = gun.slideVirtualController.OnSlideReturned + new System.Action(() => RecordSlideReturned());
            }
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

        public void InsertMagState(CartridgeData cartridgeData, MagazineData magazineData, int count)
        {
            MagazineState magazineState = new MagazineState()
            {
                cartridgeData = cartridgeData,
                magazineData = magazineData
            };
            magazineState.Initialize(cartridgeData, count);
            gun.MagazineState = magazineState;
            gun.UpdateMagazineArt();
        }

        public void RemoveMagState()
        {
            gun.MagazineState = null;
        }

        public void RecordSlideGrabbed()
        {
            RecordAction(new System.Action(() => gun.slideVirtualController.OnSlideGrabbed.Invoke()));
        }
        public void RecordSlideReleased()
        {
            RecordAction(new System.Action(() => gun.slideVirtualController.OnSlideReleased.Invoke()));
        }
        public void RecordSlidePulled()
        {
            RecordAction(new System.Action(() => gun.slideVirtualController.OnSlidePulled.Invoke()));
        }
        public void RecordSlideUpdate(float perc)
        {
            RecordAction(new System.Action(() => gun.slideVirtualController.OnSlideUpdate.Invoke(perc)));
        }
        public void RecordSlideReturned()
        {
            RecordAction(new System.Action(() => gun.slideVirtualController.OnSlideReturned.Invoke()));
        }
    }
}
