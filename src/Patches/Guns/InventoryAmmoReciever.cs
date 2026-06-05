using NEP.MonoDirector.Core;
using NEP.MonoDirector.Archetypes;

using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;
using NEP.MonoDirector.Archetypes.Proxy;

namespace NEP.MonoDirector.Patches.Guns;

public static class MagazinePatches
{
    [HarmonyLib.HarmonyPatch(typeof(Magazine), nameof(Magazine.OnGrab))]
    public static class OnGrab
    {
        public static void Postfix(Hand hand)
        {
            if (Director.PlayState != State.PlayState.Recording)
            {
                return;
            }

            HandReciever receiver = null;

            if (hand.AttachedReceiver == null)
            {
                receiver = hand.HoveringReceiver;
            }
            else
            {
                receiver = hand.AttachedReceiver;
            }
            
            var poolee = receiver.Host.Rb.GetComponent<MarrowEntity>();
            PropBuilder.BuildProp(poolee);

            // HACK:
            // Only show the magazine when it's being grabbed.
            // Insert two keyframes: one inactive and one active.
            if (!poolee.TryGetComponent(out PropProxy proxy))
                return;

            proxy.Prop.RecordActionAtTime((byte)ActionType.Hide, () => proxy.gameObject.SetActive(false), 0f);
            proxy.Prop.RecordAction((byte)ActionType.Show, () => proxy.gameObject.SetActive(true));
        }
    }
}