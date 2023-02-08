using NEP.MonoDirector.Actors;

using SLZ.Interaction;
using SLZ.Marrow.Pool;

namespace NEP.MonoDirector.Patches
{
    internal static class SpawnGun
    {
        [HarmonyLib.HarmonyPatch(typeof(SLZ.Props.SpawnGun), nameof(SLZ.Props.SpawnGun.OnFire))]
        internal static class OnFire
        {
            internal static bool Prefix(SLZ.Props.SpawnGun __instance)
            {
                if (!Settings.World.spawnGunProps)
                {
                    return true;
                }
                else
                {
                    var hitRigidbody = __instance._hitInfo.rigidbody;
                    var poolee = hitRigidbody?.GetComponent<AssetPoolee>();

                    if(__instance._selectedMode == SLZ.Props.UtilityModes.SPAWNER)
                    {
                        PropBuilder.BuildProp(poolee);
                    }
                    else
                    {
                        PropBuilder.RemoveProp(poolee);
                    }

                    return false;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SLZ.Props.SpawnGun), nameof(SLZ.Props.SpawnGun.OnHandAttachedUpdate))]
        internal static class OnHandAttachedUpdate
        {
            internal static void Postfix(SLZ.Props.SpawnGun __instance, Hand hand)
            {
                if (!Settings.World.spawnGunProps)
                {
                    __instance.placerPreivewRenderer.enabled = true;
                    __instance.placerPreviewBoundsArt.SetActive(true);

                    return;
                }

                __instance.placerPreivewRenderer.enabled = false;
                __instance.placerPreviewBoundsArt.SetActive(false);
            }
        }
    }
}
