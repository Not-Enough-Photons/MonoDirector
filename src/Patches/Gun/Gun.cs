using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;

using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Events;

namespace NEP.MonoDirector.Patches;

internal static class GunPatches
{
    internal static class GunFirePatch
    {
        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnFire))]
        internal static void Postfix(Gun __instance)
        {
            MarrowEntity entity = __instance.entity;

            if (!PropBuilder.EntityTable.TryGetValue(entity, out var prop))
                return;

            if (!prop.Armed)
                return;

            GunFirePacket packet = new GunFirePacket(__instance);
            prop.EnqueueEvent(packet);
        }
    }
}
