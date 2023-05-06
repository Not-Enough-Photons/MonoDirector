using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;
using static MelonLoader.MelonLogger;

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
                gunProp.RecordAction(gunProp.GunFakeFire);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SLZ.Props.Weapons.Gun), nameof(SLZ.Props.Weapons.Gun.PlayAnimationState))]
        internal static class PlayAnimationState
        {
            internal static void Postfix(SLZ.Props.Weapons.Gun __instance, SLZ.Props.Weapons.Gun.AnimationStates state, float startPerc)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp.RecordAction(() => __instance.SetAnimationState(state, perc));
            }
        }
    }
}
