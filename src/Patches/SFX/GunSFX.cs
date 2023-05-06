using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.Patches
{
    internal static class GunSFX
    {
        [HarmonyLib.HarmonyPatch(typeof(SLZ.SFX.GunSFX), nameof(SLZ.SFX.GunSFX.MagazineInsert))]
        internal static class MagazineInsert
        {
            internal static void Postfix(SLZ.SFX.GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.MagazineInsert);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SLZ.SFX.GunSFX), nameof(SLZ.SFX.GunSFX.MagazineDrop))]
        internal static class MagazineDrop
        {
            internal static void Postfix(SLZ.SFX.GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.MagazineDrop);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SLZ.SFX.GunSFX), nameof(SLZ.SFX.GunSFX.SlidePull))]
        internal static class SlidePull
        {
            internal static void Postfix(SLZ.SFX.GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.SlidePull);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SLZ.SFX.GunSFX), nameof(SLZ.SFX.GunSFX.SlideRelease))]
        internal static class SlideRelease
        {
            internal static void Postfix(SLZ.SFX.GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.SlideRelease);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SLZ.SFX.GunSFX), nameof(SLZ.SFX.GunSFX.SlideLock))]
        internal static class SlideLock
        {
            internal static void Postfix(SLZ.SFX.GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.SlideLock);
            }
        }
    }
}
