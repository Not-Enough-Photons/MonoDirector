using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.Patches
{
    internal static class Gun
    {
        [HarmonyLib.HarmonyPatch(typeof(SLZ.Props.Weapons.Gun), nameof(SLZ.Props.Weapons.Gun.OnFire))]
        internal static class OnFire
        {
            internal static void Postfix(SLZ.Props.Weapons.Gun __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();

                if (gunProp != null)
                {
                    gunProp.RecordAction(Recorder.instance.RecordTick, gunProp.GunFakeFire);
                }
            }
        }
    }
}
