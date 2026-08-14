using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;
using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Events;
using NEP.MonoDirector.Events.Audio;
using UnityEngine;

namespace NEP.MonoDirector.Patches.Impact;

[HarmonyLib.HarmonyPatch(typeof(ImpactSFX), nameof(ImpactSFX.ImpactSound))]
public static class ImpactSFXPatches
{
    public static void Postfix(ImpactSFX __instance, Collision c)
    {
        MarrowEntity entity = __instance._host.Body.Entity;

        if (!PropBuilder.EntityTable.TryGetValue(entity, out var prop))
            return;

        if (!prop.Armed)
            return;

        AudioImpactPacket packet = new AudioImpactPacket(
            __instance.impactHard.ToList(),
            __instance.transform.position,
            1f,
            1f,
            1f
        );
        
        prop.EnqueueEvent(packet);
    }
}