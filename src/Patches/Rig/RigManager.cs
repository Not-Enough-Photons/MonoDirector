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

                Recorder.instance.SetActor(newAvatar);
                var lastActor = Recorder.instance.LastActor;

                lastActor.CloneAvatar();
                Recorder.instance.ActiveActors.Add(lastActor);
            }
        }
    }
}
