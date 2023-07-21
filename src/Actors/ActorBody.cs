using System.Collections.Generic;

using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using SLZ.Combat;
using SLZ.Interaction;
using SLZ.Rig;
using SLZ.Vehicle;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    public class ActorBody
    {
        public struct ActorBodyPart
        {
            public void Attach(Actor actor, HumanBodyBones bone)
            {
                part.transform.parent = actor.ClonedAvatar.animator.GetBoneTransform(bone);
                part.transform.localPosition = part.transform.parent.localPosition;
                part.transform.rotation = part.transform.parent.rotation;
            }

            public GameObject part;
            public MeshCollider meshCollider;
        }

        public ActorBody(Actor actor, PhysicsRig physicsRig)
        {
            Transform actorHead = actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.Head);
            Transform actorChest = actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.UpperChest);
            Transform actorSpine = actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.Spine);
            Transform actorHips = actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.Hips);
            Transform actorLeftHand = actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.LeftHand);
            Transform actorRightHand = actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.RightHand);

            head = new GameObject("Head");
            chest = new GameObject("Chest");
            spine = new GameObject("Spine");
            hips = new GameObject("Hips");
            leftHand = new GameObject("LeftHand");
            rightHand = new GameObject("RightHand");

            headCollider = head.AddComponent<MeshCollider>();
            chestCollider = chest.AddComponent<MeshCollider>();
            spineCollider = spine.AddComponent<MeshCollider>();
            hipCollider = hips.AddComponent<MeshCollider>();
            leftHandCollider = leftHand.AddComponent<BoxCollider>();
            rightHandCollider = rightHand.AddComponent<BoxCollider>();

            ImpactProperties headVFX = head.AddComponent<ImpactProperties>();
            ImpactProperties chestVFX = chest.AddComponent<ImpactProperties>();
            ImpactProperties spineVFX = spine.AddComponent<ImpactProperties>();
            ImpactProperties hipVFX = hips.AddComponent<ImpactProperties>();
            ImpactProperties leftHandVFX = leftHand.AddComponent<ImpactProperties>();
            ImpactProperties rightHandVFX = rightHand.AddComponent<ImpactProperties>();

            headCollider.sharedMesh = physicsRig.torso.cHead.sharedMesh;
            chestCollider.sharedMesh = physicsRig.torso.cChest.sharedMesh;
            spineCollider.sharedMesh = physicsRig.torso.cSpine.sharedMesh;
            hipCollider.sharedMesh = physicsRig.torso.cPelvis.sharedMesh;

            leftHandCollider.size = physicsRig.leftHand.physHand.fingersCol.size;
            rightHandCollider.size = physicsRig.rightHand.physHand.fingersCol.size;

            head.transform.parent = actorHead;
            chest.transform.parent = actorChest;
            spine.transform.parent = actorSpine;
            hips.transform.parent = actorHips;
            leftHand.transform.parent = actorLeftHand;
            rightHand.transform.parent = actorRightHand;

            head.transform.localPosition = Vector3.zero;
            chest.transform.localPosition = Vector3.zero;
            spine.transform.localPosition = Vector3.zero;
            hips.transform.localPosition = Vector3.zero;
            leftHand.transform.localPosition = Vector3.zero;
            rightHand.transform.localPosition = Vector3.zero;

            head.transform.rotation = Quaternion.identity;
            chest.transform.rotation = Quaternion.identity;
            spine.transform.rotation = Quaternion.identity;
            hips.transform.rotation = Quaternion.identity;
            leftHand.transform.rotation = Quaternion.identity;
            rightHand.transform.rotation = Quaternion.identity;

            ImpactPropertiesManager vfxManager = physicsRig.GetComponent<ImpactPropertiesManager>();

            headVFX.surfaceData = vfxManager.surfaceData;
            chestVFX.surfaceData = vfxManager.surfaceData;
            spineVFX.surfaceData = vfxManager.surfaceData;
            hipVFX.surfaceData = vfxManager.surfaceData;
            leftHandVFX.surfaceData = vfxManager.surfaceData;
            rightHandVFX.surfaceData = vfxManager.surfaceData;
        }

        public void AllowCollisions(bool allow)
        {
            headCollider.enabled = allow;
            chestCollider.enabled = allow;
            spineCollider.enabled = allow;
            hipCollider.enabled = allow;
            leftHandCollider.enabled = allow;
            rightHandCollider.enabled = allow;
        }

        public void Delete()
        {
            GameObject.Destroy(head);
            GameObject.Destroy(chest);
            GameObject.Destroy(spine);
            GameObject.Destroy(hips);
            GameObject.Destroy(leftHand);
            GameObject.Destroy(rightHand);
        }

        private GameObject head;
        private GameObject chest;
        private GameObject spine;
        private GameObject hips;
        private GameObject leftHand;
        private GameObject rightHand;

        private MeshCollider headCollider;
        private MeshCollider chestCollider;
        private MeshCollider spineCollider;
        private MeshCollider hipCollider;
        private BoxCollider leftHandCollider;
        private BoxCollider rightHandCollider;
    }
}
