using UnityEngine;
using NEP.MonoDirector.Core;

using Il2CppSLZ.Marrow;
using NEP.MonoDirector.Archetypes;
using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Patches;

internal static class RigManagerPatches
{
    [HarmonyLib.HarmonyPatch(typeof(RigManager), nameof(RigManager.SwitchAvatar))]
    internal static class SwitchAvatar
    {
        internal static void Postfix(MarrowAvatar newAvatar)
        {
            if(Director.PlayState != State.PlayState.Recording)
            {
                return;
            }

            var activeActor = Recorder.Instance.ActiveActor;
            activeActor.RecordAction((byte)Actor.ActionType.ActorSwitchAvatar, () => activeActor.SwitchToActor(activeActor));
            activeActor.UpdateClone();
            Recorder.Instance.ActiveActors.Add(activeActor);
            Recorder.Instance.SetActor(newAvatar);
        }
    }
}
