using SLZ.VRMK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    public static class SkeletonCloner
    {
        public static void Clone(ref Transform[] bones, Animator animator)
        {
            bones = new Transform[(int)HumanBodyBones.LastBone];

            for (int i = 0; i < bones.Length; i++)
            {
                var currentBone = (HumanBodyBones)i;
                var boneTransform = animator.GetBoneTransform(currentBone);
                bones[i] = boneTransform;
            }
        }

        public static void SwapBone(ref Transform first, ref Transform second)
        {
            Transform temp = first;
            first = second;
            second = temp;
        }

        public static void SwapSkeleton(ref Transform[] first, ref Transform[] second)
        {
            Transform[] temp = first;
            first = second;
            second = temp;
        }
    }
}
