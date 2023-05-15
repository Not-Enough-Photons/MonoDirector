using SLZ.Interaction;

using NEP.MonoDirector.Actors;
using UnityEngine;

namespace NEP.MonoDirector.Patches
{
    internal static class SimpleGripEvents
    {
        [HarmonyLib.HarmonyPatch(typeof(global::SimpleGripEvents), nameof(global::SimpleGripEvents.OnAttachedDelegate))]
        internal static class OnAttachedDelegate
        {
            internal static void Postfix(global::SimpleGripEvents __instance, Hand hand)
            {
               if(__instance.GetComponent<GripEventListener>() == null)
               {
                    var listener = __instance.gameObject.AddComponent<GripEventListener>();
                    listener.SetProp(__instance.Grips[0].Host.Rb.GetComponent<Prop>());
               }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(global::SimpleGripEvents), nameof(global::SimpleGripEvents.OnDetachedDelegate))]
        internal static class OnDetachedDelegate
        {
            internal static void Postfix(global::SimpleGripEvents __instance, Hand hand)
            {
                GripEventListener listener = __instance.GetComponent<GripEventListener>();

                if (listener != null)
                {
                    GameObject.Destroy(listener);
                }
            }
        }
    }
}
