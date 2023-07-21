using UnityEngine;

namespace NEP.MonoDirector.Patches
{
    internal static class PlayerAvatarArtPatches
    {
        [HarmonyLib.HarmonyPatch(typeof(SLZ.VRMK.PlayerAvatarArt), nameof(SLZ.VRMK.PlayerAvatarArt.UpdateAvatarHead))]
        internal static class UpdateAvatarHead
        {
            internal static Vector3 preTransformHead;
            internal static Vector3 postTransformHead;
            
            internal static Vector3 calculatedHeadOffset;
            
            internal static void Prefix(SLZ.VRMK.PlayerAvatarArt __instance)
            {
                Transform head = __instance._rigManager.avatar.animator.GetBoneTransform(HumanBodyBones.Head);
                preTransformHead = head.position;
            }

            internal static void Postfix(SLZ.VRMK.PlayerAvatarArt __instance)
            {
                Transform head = __instance._rigManager.avatar.animator.GetBoneTransform(HumanBodyBones.Head);
                postTransformHead = head.position;

                calculatedHeadOffset = preTransformHead - postTransformHead;
            }
        }
    }
}