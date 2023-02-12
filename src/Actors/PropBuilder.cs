using static MelonLoader.MelonLogger;

using NEP.MonoDirector.Core;

using UnityEngine;
using SLZ.VFX;

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
                Main.Logger.Msg($"Adding gun component to {gameObject.name}");

                var actorProp = gameObject.AddComponent<ActorGunProp>();
                actorProp.SetRigidbody(rigidbody);
                actorProp.SetGun(gameObject.GetComponent<SLZ.Props.Weapons.Gun>());
                Director.instance.RecordingProps.Add(actorProp);

                vfxBlip?.CallSpawnEffect();
                return;
            }

            if (ActorProp.EligibleWithType<SLZ.Props.ObjectDestructable>(rigidbody))
            {
                Main.Logger.Msg($"Adding destructable component to {gameObject.name}");

                var destructableProp = gameObject.AddComponent<ActorBreakableProp>();
                destructableProp.SetRigidbody(rigidbody);
                destructableProp.SetBreakableObject(gameObject.GetComponent<SLZ.Props.ObjectDestructable>());

                Director.instance.RecordingProps.Add(destructableProp);

                vfxBlip?.CallSpawnEffect();
                return;
            }

            if (ActorProp.IsActorProp(rigidbody))
            {
                Main.Logger.Msg($"Adding prop component to {rigidbody.name}");

                var actorProp = gameObject.AddComponent<ActorProp>();
                actorProp.SetRigidbody(rigidbody);
                Director.instance.RecordingProps.Add(actorProp);

                vfxBlip?.CallSpawnEffect();
            }
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
                GameObject.Destroy(prop);
                vfxBlip?.CallDespawnEffect();
            }
        }
    }
}
