using SLZ.Props.Weapons;
using SLZ.Interaction;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Actors;
using SLZ.Marrow.Pool;

namespace NEP.MonoDirector.Patches.Guns
{
    public static class Magazine
    {
        [HarmonyLib.HarmonyPatch(typeof(SLZ.Props.Weapons.Magazine), nameof(SLZ.Props.Weapons.Magazine.OnGrab))]
        public static class OnGrab
        {
            public static void Postfix(Hand hand)
            {
                if (Director.PlayState == State.PlayState.Recording)
                {
                    HandReciever reciever = hand.AttachedReceiver;
                    var poolee = reciever.Host.Rb.GetComponent<AssetPoolee>();
                    PropBuilder.BuildProp(poolee);
                }
            }
        }
    }
}