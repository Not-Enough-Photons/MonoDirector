using static MelonLoader.MelonLogger;

using NEP.MonoDirector.Core;

using UnityEngine;
using SLZ.VFX;
using NEP.MonoDirector.Patches.Guns;
using SLZ.Vehicle;
using static NEP.MonoDirector.Patches.GunSFX;

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
            bool isProp = gameObject.GetComponent<Prop>() != null;

            if (!hasRigidbody)
            {
                return;
            }

            if (isProp)
            {
                return;
            }

            var vfxBlip = rigidbody.GetComponent<Blip>();

            if (Prop.EligibleWithType<SLZ.Props.Weapons.Gun>(rigidbody))
            {
                Main.Logger.Msg($"Adding gun component to {gameObject.name}");

                var actorProp = gameObject.AddComponent<GunProp>();
                actorProp.SetRigidbody(rigidbody);
                actorProp.SetGun(gameObject.GetComponent<SLZ.Props.Weapons.Gun>());
                Director.instance.RecordingProps.Add(actorProp);

                vfxBlip?.CallSpawnEffect();

                Events.OnPropCreated?.Invoke(actorProp);
                return;
            }

            if (Prop.EligibleWithType<SLZ.Props.ObjectDestructable>(rigidbody))
            {
                Main.Logger.Msg($"Adding destructable component to {gameObject.name}");

                var destructableProp = gameObject.AddComponent<BreakableProp>();
                destructableProp.SetRigidbody(rigidbody);
                destructableProp.SetBreakableObject(gameObject.GetComponent<SLZ.Props.ObjectDestructable>());

                Director.instance.RecordingProps.Add(destructableProp);

                vfxBlip?.CallSpawnEffect();

                Events.OnPropCreated?.Invoke(destructableProp);
                return;
            }

            if (Prop.EligibleWithType<SLZ.Props.Weapons.Magazine>(rigidbody))
            {
                Main.Logger.Msg($"Adding magazine component to {gameObject.name}");

                var magazineProp = gameObject.AddComponent<Prop>();
                magazineProp.SetRigidbody(rigidbody);

                Director.instance.RecordingProps.Add(magazineProp);

                vfxBlip?.CallSpawnEffect();

                Events.OnPropCreated?.Invoke(magazineProp);
                return;
            }

            if (Prop.EligibleWithType<Atv>(rigidbody))
            {
                Main.Logger.Msg($"Adding vehicle component to {gameObject.name}");

                var vehicle = gameObject.AddComponent<TrackedVehicle>();
                vehicle.SetRigidbody(rigidbody);
                vehicle.SetVehicle(rigidbody.GetComponent<Atv>());

                Director.instance.RecordingProps.Add(vehicle);
                vfxBlip?.CallSpawnEffect();

                Events.OnPropCreated?.Invoke(vehicle);
                return;
            }

            if (Prop.IsActorProp(rigidbody))
            {
                Main.Logger.Msg($"Adding prop component to {rigidbody.name}");

                var actorProp = gameObject.AddComponent<Prop>();
                actorProp.SetRigidbody(rigidbody);
                Director.instance.RecordingProps.Add(actorProp);

                vfxBlip?.CallSpawnEffect();

                Events.OnPropCreated?.Invoke(actorProp);
            }
        }

        public static void RemoveProp(SLZ.Marrow.Pool.AssetPoolee pooleeObject)
        {
            var gameObject = pooleeObject.gameObject;
            var vfxBlip = gameObject.GetComponent<Blip>();

            Prop actorProp = gameObject.GetComponent<Prop>();
            bool isProp = actorProp != null;

            if (isProp && Director.PlayState == State.PlayState.Stopped)
            {
                MelonLoader.MelonLogger.Msg($"Removing component from {gameObject.name}");

                var prop = actorProp;
                prop.InteractableRigidbody.isKinematic = false;
                Director.instance.RecordingProps.Remove(prop);
                GameObject.Destroy(prop);
                vfxBlip?.CallDespawnEffect();

                Events.OnPropRemoved?.Invoke(prop);
            }
        }
    }
}
