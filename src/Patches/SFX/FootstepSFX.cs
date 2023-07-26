using NEP.MonoDirector.Core;
using NEP.MonoDirector.Audio;

using UnityEngine;

using BoneLib.Nullables;

namespace NEP.MonoDirector.Patches
{
    internal class FootstepSFX
    {
        [HarmonyLib.HarmonyPatch(typeof(SLZ.SFX.FootstepSFX), nameof(SLZ.SFX.FootstepSFX.PlayStep))]
        internal static class PlayStep
        {
            internal static void Postfix(SLZ.SFX.FootstepSFX __instance, float velocitySqr)
            {
                Main.Logger.Msg(velocitySqr.ToString());

                var activeActor = Recorder.instance.ActiveActor;

                if(activeActor == null)
                {
                    return;
                }

                activeActor.RecordAction(() => PlayFootstep(__instance, velocitySqr));
            }

            internal static void PlayFootstep(SLZ.SFX.FootstepSFX footstep, float velocitySqr)
            {
                var activeActor = Recorder.instance.ActiveActor;
            }
        }
    }
}
