using Il2CppSLZ.Marrow;
using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Events;
using UnityEngine;

namespace NEP.MonoDirector.Patches.Rig;

[HarmonyLib.HarmonyPatch(typeof(HandSFX), nameof(HandSFX.Grab))]
public static class GrabSFXPatch
{
    public static void Postfix(HandSFX __instance, float volumeMult)
    {
        Transform root = __instance.transform.root;
        RigManager rig = RigManager.Cache.Get(root.gameObject);

        if (!ActorBuilder.AvatarTable.TryGetValue(rig.avatar.gameObject, out Actor actor))
            return;

        if (!actor.Armed)
            return;

        HandGrabPacket packet = new HandGrabPacket(
            __instance.grab.ToList(), 
            __instance.transform.position, 
            1f, 
            1f, 
            1f);
        
        actor.EnqueueEvent(packet);
    }
}