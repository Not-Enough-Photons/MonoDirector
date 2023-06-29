using UnityEngine;
using SLZ.Rig;
using NEP.MonoDirector.Core;

using Avatar = SLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Patches
{
    internal static class RigManager
    {
        [HarmonyLib.HarmonyPatch(typeof(SLZ.Rig.RigManager), nameof(SLZ.Rig.RigManager.SwitchAvatar))]
        internal static class SwitchAvatar
        {
            internal static void Postfix(Avatar newAvatar)
            {
                if(Director.PlayState != State.PlayState.Recording)
                {
                    return;
                }

                var activeActor = Recorder.instance.ActiveActor;
                activeActor.RecordAction(new System.Action(() => activeActor.SwitchToActor(activeActor)));
                activeActor.CloneAvatar();
                Recorder.instance.ActiveActors.Add(activeActor);
                Recorder.instance.SetActor(newAvatar);
            }
        }
    }
}
