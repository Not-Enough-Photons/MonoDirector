using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;

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
                var prop = destructable.GetComponent<BreakableProp>();

                if(prop != null && Director.PlayState == State.PlayState.Recording)
                {
                    prop.RecordDestructionEvent(Recorder.instance.RecordingTime, prop.DestructionEvent);
                }
            }
        }
    }
}
