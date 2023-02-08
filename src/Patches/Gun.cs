using NEP.MonoDirector.Actors;

namespace NEP.MonoDirector.Patches
{
    internal static class Gun
    {
        [HarmonyLib.HarmonyPatch(typeof(SLZ.Props.Weapons.Gun), nameof(SLZ.Props.Weapons.Gun.OnFire))]
        internal static class OnFire
        {
            internal static void Postfix(SLZ.Props.Weapons.Gun __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<ActorGunProp>();

                if (gunProp != null)
                {
                    if (Director.instance.playState == State.PlayState.Recording)
                    {
                        gunProp.RecordGunShot(Director.instance.WorldTick, gunProp.GunFakeFire);
                    }
                }
            }
        }
    }
}
