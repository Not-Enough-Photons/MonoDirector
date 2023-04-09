using NEP.MonoDirector.Core;

using UnityEngine;

using SLZ.VFX;
using SLZ.Vehicle;

namespace NEP.MonoDirector.Actors
{
    public static class PropBuilder
    {
        public static void BuildProp(SLZ.Marrow.Pool.AssetPoolee pooleeObject)
        {
            if (pooleeObject == null)
            {
                return;
            }

            var gameObject = pooleeObject.gameObject;
            var rigidbody = gameObject.GetComponent<Rigidbody>();

            bool hasRigidbody = rigidbody != null;
            bool isProp = gameObject.GetComponent<ActorProp>() != null;

            if (!hasRigidbody)
            {
                return;
            }

            if (isProp)
            {
                return;
            }

            var vfxBlip = rigidbody.GetComponent<Blip>();

            if (ActorProp.EligibleWithType<SLZ.Props.Weapons.Gun>(rigidbody))
            {
                BuildGunProp(rigidbody);
                return;
            }

            if (ActorProp.EligibleWithType<SLZ.Props.ObjectDestructable>(rigidbody))
            {
                BuildDestructibleProp(rigidbody);
                return;
            }

            if (ActorProp.EligibleWithType<SLZ.Props.Weapons.Magazine>(rigidbody))
            {
                BuildMagazineProp(rigidbody);
                return;
            }

            if (ActorProp.EligibleWithType<Atv>(rigidbody))
            {
                BuildVehicle(rigidbody);
                return;
            }

            if (ActorProp.IsActorProp(rigidbody))
            {
                Main.Logger.Msg($"Adding prop component to {rigidbody.name}");

                var actorProp = new ActorProp();
                actorProp.Pair(rigidbody);
            }
            
            vfxBlip?.CallSpawnEffect();
        }

        public static void RemoveProp(SLZ.Marrow.Pool.AssetPoolee pooleeObject)
        {
            var gameObject = pooleeObject.gameObject;
            var vfxBlip = gameObject.GetComponent<Blip>();

            ActorProp actorProp = gameObject.GetComponent<ActorProp>();
            bool isProp = actorProp != null;

            if (isProp && Director.PlayState == State.PlayState.Stopped)
            {
                MelonLoader.MelonLogger.Msg($"Removing component from {gameObject.name}");

                var prop = actorProp;
                prop.InteractableRigidbody.isKinematic = false;
                Director.instance.RecordingProps.Remove(prop);
                vfxBlip?.CallDespawnEffect();
            }
        }

        internal static void BuildGunProp(Rigidbody prop)
        {
            GameObject gameObject = prop.gameObject;

            var actorProp = new ActorGunProp();

            actorProp.Pair(prop);
            actorProp.SetGun(gameObject.GetComponent<SLZ.Props.Weapons.Gun>());
        }

        internal static void BuildDestructibleProp(Rigidbody prop)
        {
            GameObject gameObject = prop.gameObject;

            var destructableProp = new ActorBreakableProp();

            destructableProp.Pair(prop);
            destructableProp.SetBreakableObject(gameObject.GetComponent<SLZ.Props.ObjectDestructable>());
        }

        internal static void BuildMagazineProp(Rigidbody prop)
        {
            GameObject gameObject = prop.gameObject;

            var magazineProp = new ActorProp();

            magazineProp.Pair(prop);
        }

        internal static void BuildVehicle(Rigidbody prop)
        {
            GameObject gameObject = prop.gameObject;

            var vehicle = new ActorVehicle();

            vehicle.Pair(prop);
            vehicle.SetVehicle(prop.GetComponent<Atv>());
        }
    }
}
