using static MelonLoader.MelonLogger;

using NEP.MonoDirector.Core;

using UnityEngine;
using SLZ.VFX;

namespace NEP.MonoDirector.Actors
{
    public static class ActorNPCBuilder
    {
        public static void BuildNPCActor(SLZ.Marrow.Pool.AssetPoolee pooleeObject)
        {
            ActorNPC test = new ActorNPC(pooleeObject.transform);
            Director.instance.NPCCast.Add(test);
        }

        public static void RemoveNPCActor(SLZ.Marrow.Pool.AssetPoolee pooleeObject)
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
