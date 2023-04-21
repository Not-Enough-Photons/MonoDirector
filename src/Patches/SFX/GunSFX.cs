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

                if(Director.PlayState == State.PlayState.Recording)
                {
                    gunProp?.RecordAction(Recorder.instance.RecordTick, gunProp.Gun.gunSFX.MagazineInsert);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SLZ.SFX.GunSFX), nameof(SLZ.SFX.GunSFX.SlidePull))]
        internal static class SlidePull
        {
            internal static void Postfix(SLZ.SFX.GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();

                if(Director.PlayState == State.PlayState.Recording)
                {
                    gunProp?.RecordAction(Recorder.instance.RecordTick, gunProp.Gun.gunSFX.SlidePull);
                }
            }
        }
    }
}
