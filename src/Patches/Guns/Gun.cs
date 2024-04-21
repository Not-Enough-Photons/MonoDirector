using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;

using SLZ.Marrow.Data;
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
                gunProp?.RecordAction(new System.Action(() => gunProp.GunFakeFire()));
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SLZ.Props.Weapons.Gun), nameof(SLZ.Props.Weapons.Gun.SetAnimationState))]
        internal static class PlayAnimationState
        {
            internal static void Postfix(SLZ.Props.Weapons.Gun __instance, SLZ.Props.Weapons.Gun.AnimationStates state, float perc)
            {
                /*var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp.RecordAction(() => __instance.SetAnimationState(state, perc));*/
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SLZ.Props.Weapons.Gun), nameof(SLZ.Props.Weapons.Gun.OnMagazineInserted))]
        internal static class OnMagazineInserted
        {
            internal static void Postfix(SLZ.Props.Weapons.Gun __instance)
            {
                if (__instance._magState != null)
                {
                    var gunProp = __instance.gameObject.GetComponent<GunProp>();
                    int count = __instance._magState.AmmoCount;
                    CartridgeData cartridgeData = __instance._magState.cartridgeData;
                    MagazineData magazineData = __instance._magState.magazineData;
                    gunProp?.RecordAction(new System.Action(() => gunProp.InsertMagState(cartridgeData, magazineData, count)));
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SLZ.Props.Weapons.Gun), nameof(SLZ.Props.Weapons.Gun.OnMagazineRemoved))]
        internal static class OnMagazineRemoved
        {
            internal static void Postfix(SLZ.Props.Weapons.Gun __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(new System.Action(() => gunProp.RemoveMagState()));
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SLZ.Props.Weapons.Gun), nameof(SLZ.Props.Weapons.Gun.UpdateArt))]
        internal static class UpdateArt
        {
            internal static void Postfix(SLZ.Props.Weapons.Gun __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(new System.Action(() => __instance.UpdateArt()));
            }
        }
    }
}
