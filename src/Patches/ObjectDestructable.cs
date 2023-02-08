using MelonLoader;
using NEP.MonoDirector;
using NEP.MonoDirector.Actors;
using SLZ.Marrow.Data;
using System.Runtime.InteropServices;
using UnityEngine;

namespace NEP.MonoDirector.Patches
{
    internal class ObjectDestructable
    {
        [HarmonyLib.HarmonyPatch(typeof(SLZ.Props.ObjectDestructable), nameof(SLZ.Props.ObjectDestructable.Awake))]
        internal static class TakeDamage
        {
            static void Postfix(SLZ.Props.ObjectDestructable __instance)
            {
                __instance.OnDestruction += new System.Action<SLZ.Props.ObjectDestructable>(OnObjectDestroyed);
            }

            static void OnObjectDestroyed(SLZ.Props.ObjectDestructable destructable)
            {
                var prop = destructable.GetComponent<ActorBreakableProp>();

                if(prop != null && Director.instance.playState == State.PlayState.Recording)
                {
                    prop.RecordDestructionEvent(Director.instance.WorldTick, prop.DestructionEvent);
                }
            }
        }
    }
}
