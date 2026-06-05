using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Core;

using Il2CppSLZ.Marrow;
using NEP.MonoDirector.Archetypes.Proxy;

namespace NEP.MonoDirector.Patches;

internal static class ObjectDestructiblePatches
{
    [HarmonyLib.HarmonyPatch(typeof(ObjectDestructible), nameof(ObjectDestructible.Awake))]
    internal static class TakeDamage
    {
        static void Postfix(ObjectDestructible __instance)
        {
            __instance.OnDestruction += new System.Action<ObjectDestructible>(OnObjectDestroyed);
        }

        static void OnObjectDestroyed(ObjectDestructible destructable)
        {
            if (destructable.TryGetComponent(out PropProxy proxy))
                return;
        }
    }
}
