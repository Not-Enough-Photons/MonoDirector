using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    public class ActorSkeleton
    {
        public ActorSkeleton(Animator animator)
        {
            head = animator.GetBoneTransform(HumanBodyBones.Head);
            neck = animator.GetBoneTransform(HumanBodyBones.Neck);
            upperChest = animator.GetBoneTransform(HumanBodyBones.UpperChest);
            chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            hips = animator.GetBoneTransform(HumanBodyBones.Hips);

            leftShoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
            leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            leftLowerArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            leftThumbProximal = animator.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
            leftThumbIntermediate = animator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
            leftThumbDistal = animator.GetBoneTransform(HumanBodyBones.LeftThumbDistal);
            leftIndexProximal = animator.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
            leftIndexIntermediate = animator.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate);
            leftIndexDistal = animator.GetBoneTransform(HumanBodyBones.LeftIndexDistal);
            leftMiddleProximal = animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
            leftMiddleIntermediate = animator.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate);
            leftMiddleDistal = animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);
            leftRingProximal = animator.GetBoneTransform(HumanBodyBones.LeftRingProximal);
            leftRingIntermediate = animator.GetBoneTransform(HumanBodyBones.LeftRingIntermediate);
            leftRingDistal = animator.GetBoneTransform(HumanBodyBones.LeftRingDistal);
            leftLittleProximal = animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal);
            leftLittleIntermediate = animator.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate);
            leftLittleDistal = animator.GetBoneTransform(HumanBodyBones.LeftLittleDistal);

            rightShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
            rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            rightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            rightThumbProximal = animator.GetBoneTransform(HumanBodyBones.RightThumbProximal);
            rightThumbIntermediate = animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
            rightThumbDistal = animator.GetBoneTransform(HumanBodyBones.RightThumbDistal);
            rightIndexProximal = animator.GetBoneTransform(HumanBodyBones.RightIndexProximal);
            rightIndexIntermediate = animator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate);
            rightIndexDistal = animator.GetBoneTransform(HumanBodyBones.RightIndexDistal);
            rightMiddleProximal = animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
            rightMiddleIntermediate = animator.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate);
            rightMiddleDistal = animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal);
            rightRingProximal = animator.GetBoneTransform(HumanBodyBones.RightRingProximal);
            rightRingIntermediate = animator.GetBoneTransform(HumanBodyBones.RightRingIntermediate);
            rightRingDistal = animator.GetBoneTransform(HumanBodyBones.RightRingDistal);
            rightLittleProximal = animator.GetBoneTransform(HumanBodyBones.RightLittleProximal);
            rightLittleIntermediate = animator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate);
            rightLittleDistal = animator.GetBoneTransform(HumanBodyBones.RightLittleDistal);

            leftUpperLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            leftLowerLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            leftToes = animator.GetBoneTransform(HumanBodyBones.LeftToes);

            rightUpperLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            rightLowerLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
            rightToes = animator.GetBoneTransform(HumanBodyBones.RightToes);

            jaw = animator.GetBoneTransform(HumanBodyBones.Jaw);
            leftEye = animator.GetBoneTransform(HumanBodyBones.LeftEye);
            rightEye = animator.GetBoneTransform(HumanBodyBones.RightEye);

            List<Transform> bones = new List<Transform>();

            bones.Add(head);
            bones.Add(neck);
            bones.Add(upperChest);
            bones.Add(chest);
            bones.Add(spine);
            bones.Add(hips);

            bones.Add(leftShoulder);
            bones.Add(leftUpperArm);
            bones.Add(leftLowerArm);
            bones.Add(leftHand);
            bones.Add(leftThumbProximal);
            bones.Add(leftThumbIntermediate);
            bones.Add(leftThumbDistal);
            bones.Add(leftIndexProximal);
            bones.Add(leftIndexIntermediate);
            bones.Add(leftIndexDistal);
            bones.Add(leftMiddleProximal);
            bones.Add(leftMiddleIntermediate);
            bones.Add(leftMiddleDistal);
            bones.Add(leftRingProximal);
            bones.Add(leftRingIntermediate);
            bones.Add(leftRingDistal);
            bones.Add(leftLittleProximal);
            bones.Add(leftLittleIntermediate);
            bones.Add(leftLittleDistal);

            bones.Add(rightShoulder);
            bones.Add(rightUpperArm);
            bones.Add(rightLowerArm);
            bones.Add(rightHand);
            bones.Add(rightThumbProximal);
            bones.Add(rightThumbIntermediate);
            bones.Add(rightThumbDistal);
            bones.Add(rightIndexProximal);
            bones.Add(rightIndexIntermediate);
            bones.Add(rightIndexDistal);
            bones.Add(rightMiddleProximal);
            bones.Add(rightMiddleIntermediate);
            bones.Add(rightMiddleDistal);
            bones.Add(rightRingProximal);
            bones.Add(rightRingIntermediate);
            bones.Add(rightRingDistal);
            bones.Add(rightLittleProximal);
            bones.Add(rightLittleIntermediate);
            bones.Add(rightLittleDistal);

            bones.Add(leftUpperLeg);
            bones.Add(leftLowerLeg);
            bones.Add(leftFoot);
            bones.Add(leftToes);

            bones.Add(rightUpperLeg);
            bones.Add(rightLowerLeg);
            bones.Add(rightFoot);
            bones.Add(rightToes);

            bones.Add(jaw);
            bones.Add(leftEye);
            bones.Add(rightEye);

            this.bones = bones.ToArray();
        }

        public Transform[] bones { get; private set; }

        public Transform head;
        public Transform neck;
        public Transform upperChest;
        public Transform chest;
        public Transform spine;
        public Transform hips;

        public Transform leftShoulder;
        public Transform leftUpperArm;
        public Transform leftLowerArm;
        public Transform leftHand;
        public Transform leftThumbProximal;
        public Transform leftThumbIntermediate;
        public Transform leftThumbDistal;
        public Transform leftIndexProximal;
        public Transform leftIndexIntermediate;
        public Transform leftIndexDistal;
        public Transform leftMiddleProximal;
        public Transform leftMiddleIntermediate;
        public Transform leftMiddleDistal;
        public Transform leftRingProximal;
        public Transform leftRingIntermediate;
        public Transform leftRingDistal;
        public Transform leftLittleProximal;
        public Transform leftLittleIntermediate;
        public Transform leftLittleDistal;

        public Transform rightShoulder;
        public Transform rightUpperArm;
        public Transform rightLowerArm;
        public Transform rightHand;
        public Transform rightThumbProximal;
        public Transform rightThumbIntermediate;
        public Transform rightThumbDistal;
        public Transform rightIndexProximal;
        public Transform rightIndexIntermediate;
        public Transform rightIndexDistal;
        public Transform rightMiddleProximal;
        public Transform rightMiddleIntermediate;
        public Transform rightMiddleDistal;
        public Transform rightRingProximal;
        public Transform rightRingIntermediate;
        public Transform rightRingDistal;
        public Transform rightLittleProximal;
        public Transform rightLittleIntermediate;
        public Transform rightLittleDistal;

        public Transform leftUpperLeg;
        public Transform leftLowerLeg;
        public Transform leftFoot;
        public Transform leftToes;

        public Transform rightUpperLeg;
        public Transform rightLowerLeg;
        public Transform rightFoot;
        public Transform rightToes;

        public Transform jaw;
        public Transform leftEye;
        public Transform rightEye;
    }
}
