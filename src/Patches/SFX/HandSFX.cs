using NEP.MonoDirector.Core;
using UnityEngine;

using BoneLib.Nullables;

namespace NEP.MonoDirector.Patches
{
    internal class HandSFX
    {
        [HarmonyLib.HarmonyPatch(typeof(SLZ.SFX.HandSFX), nameof(SLZ.SFX.HandSFX.Grab))]
        internal static class Grab
        {
            internal static void Postfix(SLZ.SFX.HandSFX __instance)
            {
                var activeActor = Recorder.instance.ActiveActor;

                if (activeActor == null)
                {
                    return;
                }

                if(Director.PlayState == State.PlayState.Recording)
                {
                    activeActor.RecordAction(() => PlaySFX(__instance));
                }
            }

            internal static void PlaySFX(SLZ.SFX.HandSFX hand)
            {
                int rand = Random.Range(0, hand.grab.Count);

                NullableMethodExtensions.AudioPlayer_PlayAtPoint(hand.grab[rand], hand.transform.position);
            }
        }
    }
}
