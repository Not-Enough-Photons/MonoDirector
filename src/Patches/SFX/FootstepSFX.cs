using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;
using SLZ.Utilities;
using UnityEngine;

namespace NEP.MonoDirector.Patches
{
    internal class FootstepSFX
    {
        [HarmonyLib.HarmonyPatch(typeof(SLZ.SFX.FootstepSFX), nameof(SLZ.SFX.FootstepSFX.PlayStep))]
        internal static class PlayStep
        {
            internal static void Postfix(SLZ.SFX.FootstepSFX __instance, float velocitySqr)
            {
                var activeActor = Recorder.instance.ActiveActor;

                if(activeActor == null)
                {
                    return;
                }

                activeActor.CaptureAvatarAction(Recorder.instance.RecordTick, () => PlayFootstep(__instance));
            }

            internal static void PlayFootstep(SLZ.SFX.FootstepSFX footstep)
            {
                AudioPlayer.PlayAtPoint(footstep.walkConcrete, footstep.transform.position);
            }
        }
    }
}
