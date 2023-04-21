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

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class GunProp : Prop
    {
        public GunProp(IntPtr ptr) : base(ptr) { }

        public Gun Gun { get => gun; }

        private Gun gun;
        private Dictionary<int, Action> actionFrames;

        protected override void Awake()
        {
            base.Awake();

            propFrames = new List<ObjectFrame>();
            actionFrames = new Dictionary<int, Action>();
        }

        public void RecordAction(int frame, Action action)
        {
            if (Director.PlayState == State.PlayState.Recording)
            {
                if (!actionFrames.ContainsKey(frame))
                {
                    actionFrames.Add(frame, action);
                }
            }
        }

        public void GunFakeFire()
        {
            string muzzleFlashBarcode = "c1534c5a-93e8-405b-89e2-e39c466c6172";
            SpawnableCrateReference reference = new SpawnableCrateReference(muzzleFlashBarcode);

            Spawnable spawnable = new Spawnable()
            {
                crateRef = reference
            };

            AssetSpawner.Register(spawnable);
            AssetSpawner.Spawn(spawnable, gun.firePointTransform.position, gun.firePointTransform.rotation, new BoxedNullable<Vector3>(Vector3.one), false, new BoxedNullable<int>(null), null, null);

            gun.gunSFX.GunShot();

            gun.PlayAnimationState(Gun.AnimationStates.FIRE, 0f);
        }

        public void SetGun(Gun gun)
        {
            this.gun = gun;
        }

        public override void Act()
        {
            base.Act();
        }
    }
}
