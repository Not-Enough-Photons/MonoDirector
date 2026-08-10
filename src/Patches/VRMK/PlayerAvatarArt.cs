using UnityEngine;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Patches;

internal static class PlayerAvatarArtPatches
{
    [HarmonyLib.HarmonyPatch(typeof(PlayerAvatarArt), nameof(PlayerAvatarArt.UpdateAvatarHead))]
    internal static class UpdateAvatarHead
    {
        internal static Vector3 PreTransformHead;
        internal static Vector3 PostTransformHead;
        
        internal static Vector3 CalculatedHeadOffset;
        
        internal static void Prefix(PlayerAvatarArt __instance)
        {
            RigManager manager = __instance._openCtrlRig.manager;

            Transform head = manager.avatar.animator.GetBoneTransform(HumanBodyBones.Head);
            PreTransformHead = head.position;
        }

        internal static void Postfix(PlayerAvatarArt __instance)
        {
            RigManager manager = __instance._openCtrlRig.manager;

            Transform head = manager.avatar.animator.GetBoneTransform(HumanBodyBones.Head);
            PostTransformHead = head.position;

            CalculatedHeadOffset = PreTransformHead - PostTransformHead;
        }
    }
}