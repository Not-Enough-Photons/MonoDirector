using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Archetypes.Proxy;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Patches;

internal static class GunSFXPatches
{
    [HarmonyLib.HarmonyPatch(typeof(GunSFX), nameof(GunSFX.MagazineInsert))]
    internal static class MagazineInsert
    {
        internal static void Postfix(GunSFX __instance)
        {
            if (!__instance.TryGetComponent(out PropProxy proxy))
                return;

            GunProp prop = (GunProp)proxy.Prop;
            //prop?.RecordAction(prop.Gun.gunSFX.MagazineInsert);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(GunSFX), nameof(GunSFX.MagazineDrop))]
    internal static class MagazineDrop
    {
        internal static void Postfix(GunSFX __instance)
        {
            if (!__instance.TryGetComponent(out PropProxy proxy))
                return;

            GunProp prop = (GunProp)proxy.Prop;
            //prop?.RecordAction(prop.Gun.gunSFX.MagazineDrop);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(GunSFX), nameof(GunSFX.SlidePull))]
    internal static class SlidePull
    {
        internal static void Postfix(GunSFX __instance)
        {
            if (!__instance.TryGetComponent(out PropProxy proxy))
                return;

            GunProp prop = (GunProp)proxy.Prop;
            //prop?.RecordAction(prop.Gun.gunSFX.SlidePull);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(GunSFX), nameof(GunSFX.SlideRelease))]
    internal static class SlideRelease
    {
        internal static void Postfix(GunSFX __instance)
        {
            if (!__instance.TryGetComponent(out PropProxy proxy))
                return;

            GunProp prop = (GunProp)proxy.Prop;
            //prop?.RecordAction(prop.Gun.gunSFX.SlideRelease);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(GunSFX), nameof(GunSFX.SlideLock))]
    internal static class SlideLock
    {
        internal static void Postfix(GunSFX __instance)
        {
            if (!__instance.TryGetComponent(out PropProxy proxy))
                return;

            GunProp prop = (GunProp)proxy.Prop;
            //prop?.RecordAction(prop.Gun.gunSFX.SlideLock);
        }
    }
}
