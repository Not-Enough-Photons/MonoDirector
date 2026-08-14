using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Audio;
using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Events;
using UnityEngine;

namespace NEP.MonoDirector.Patches.Rig;

[HarmonyLib.HarmonyPatch(typeof(FootstepSFX), nameof(FootstepSFX.PlayStep))]
public static class FootstepSFXPatch
{
    public static void Postfix(FootstepSFX __instance, float velocitySqr)
    {
        Transform root = __instance.transform.root;
        RigManager rig = RigManager.Cache.Get(root.gameObject);

        if (!ActorBuilder.AvatarTable.TryGetValue(rig.avatar.gameObject, out Actor actor))
            return;

        if (!actor.Armed)
            return;

        FootstepPacket packet = new FootstepPacket(
            actor, 
            __instance.source.transform.position, 
            __instance.source.volume, 
            __instance.source.pitch, 
            __instance.source.spatialBlend,
            velocitySqr > 1.5f);
        
        actor.EnqueueEvent(packet);
    }
}