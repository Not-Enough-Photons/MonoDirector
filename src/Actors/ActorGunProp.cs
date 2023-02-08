using System.Collections.Generic;
using NEP.MonoDirector.Data;
using SLZ.Marrow.Data;
using SLZ.Marrow.Pool;
using SLZ.Marrow.Warehouse;
using SLZ.Props.Weapons;
using System;
using UnityEngine;
using BoneLib.Nullables;
using BoneLib;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorGunProp : ActorProp
    {
        public ActorGunProp(IntPtr ptr) : base(ptr) { }

        private Gun gun;
        private Dictionary<int, Action> gunShotFrames;

        protected override void Awake()
        {
            base.Awake();

            propFrames = new Dictionary<int, ObjectFrame>();
            gunShotFrames = new Dictionary<int, Action>();
        }

        public void GunFakeFire()
        {
            string barcode = "c1534c5a-93e8-405b-89e2-e39c466c6172";
            SpawnableCrateReference reference = new SpawnableCrateReference(barcode);

            Spawnable spawnable = new Spawnable()
            {
                crateRef = reference
            };

            AssetSpawner.Register(spawnable);
            AssetSpawner.Spawn(spawnable, gun.firePointTransform.position, gun.firePointTransform.rotation, new BoxedNullable<Vector3>(Vector3.one), false, new BoxedNullable<int>(null), null, null);

            AudioSource.PlayClipAtPoint(gun.gunSFX.fire[UnityEngine.Random.Range(0, gun.gunSFX.fire.Length)], gun.firePointTransform.position);
        }

        public void SetGun(Gun gun)
        {
            this.gun = gun;
        }

        public override void Play(int currentTick)
        {
            base.Play(currentTick);

            if (gunShotFrames.ContainsKey(currentTick))
            {
                gunShotFrames[currentTick]?.Invoke();
            }
        }

        public void RecordGunShot(int timeStamp, Action action)
        {
            if (!gunShotFrames.ContainsKey(timeStamp))
            {
                gunShotFrames.Add(timeStamp, action);
            }
        }
    }
}
