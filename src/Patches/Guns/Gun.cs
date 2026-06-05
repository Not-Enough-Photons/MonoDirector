using NEP.MonoDirector.Archetypes;

using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Data;
using NEP.MonoDirector.Archetypes.Proxy;

namespace NEP.MonoDirector.Patches;

internal static class GunPatches
{
    [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnFire))]
    internal static class OnFire
    {
        internal static void Postfix(Gun __instance)
        {
            if (!__instance.TryGetComponent(out PropProxy proxy))
                return;

            GunProp prop = (GunProp)proxy.Prop;
            prop.RecordAction((byte)ActionType.Fire, () => prop.GunFakeFire());
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.SetAnimationState))]
    internal static class PlayAnimationState
    {
        internal static void Postfix(Gun __instance, Gun.AnimationStates state, float perc)
        {
            if (!__instance.TryGetComponent(out PropProxy proxy))
                return;

            GunProp prop = (GunProp)proxy.Prop;
            prop?.RecordAction((byte)ActionType.AnimStateUpdate, () => __instance.SetAnimationState(state, perc));
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnMagazineInserted))]
    internal static class OnMagazineInserted
    {
        internal static void Postfix(Gun __instance)
        {
            if (__instance._magState != null)
            {
                if (!__instance.TryGetComponent(out PropProxy proxy))
                    return;

                GunProp prop = (GunProp)proxy.Prop;
                int count = __instance._magState.AmmoCount;
                CartridgeData cartridgeData = __instance._magState.cartridgeData;
                MagazineData magazineData = __instance._magState.magazineData;
                prop?.RecordAction((byte)ActionType.InsertMag, () => prop.InsertMagState(cartridgeData, magazineData, count));
            }
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnMagazineRemoved))]
    internal static class OnMagazineRemoved
    {
        internal static void Postfix(Gun __instance)
        {
            if (!__instance.TryGetComponent(out PropProxy proxy))
                return;

            GunProp prop = (GunProp)proxy.Prop;
            prop?.RecordAction((byte)ActionType.RemoveMag, () => prop.RemoveMagState());
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.UpdateArt))]
    internal static class UpdateArt
    {
        internal static void Postfix(Gun __instance)
        {
            if (!__instance.TryGetComponent(out PropProxy proxy))
                return;

            GunProp prop = (GunProp)proxy.Prop;
            prop?.RecordAction((byte)ActionType.UpdateArt, () => prop.UpdateArt());
        }
    }
}
