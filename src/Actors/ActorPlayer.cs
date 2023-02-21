using System;
using System.Collections.Generic;
using BoneLib;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using SLZ.Props;
using SLZ.VFX;
using SLZ.VRMK;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    public class ActorPlayer : Actor
    {
        public ActorPlayer(SLZ.VRMK.Avatar avatar) : base()
        {
            playerAvatar = avatar;
            avatarBones = GetAvatarBones(playerAvatar);
            avatarFrames = new Dictionary<int, TickFrame>();
        }

        public SLZ.VRMK.Avatar PlayerAvatar { get => playerAvatar; }
        public Transform[] AvatarBones { get => avatarBones; }

        protected Dictionary<int, TickFrame> avatarFrames;

        private SLZ.VRMK.Avatar playerAvatar;
        private SLZ.VRMK.Avatar clonedAvatar;

        private Transform[] avatarBones;
        private Transform[] clonedRigBones;

        public override void Act(int currentFrame)
        {
            base.Act(currentFrame);

            if (!CanAct(currentFrame))
            {
                return;
            }

            var actorFrame = avatarFrames[currentFrame];

            for (int i = 0; i < actorFrame.transformFrames.Count; i++)
            {
                var boneFrame = actorFrame.transformFrames[i];
                boneFrame.transform = clonedRigBones[i];

                if (boneFrame.transform == null)
                {
                    continue;
                }

                boneFrame.transform.position = boneFrame.position;
                boneFrame.transform.rotation = boneFrame.rotation;
            }
        }

        /// <summary>
        /// Records the actor's bones, positons, and rotations for this frame.
        /// </summary>
        /// <param name="index">The frame to record the bones.</param>
        public override void RecordFrame()
        {
            avatarFrames.Add(recordedTicks++, new TickFrame(CaptureBoneFrames(avatarBones)));
        }

        public void CaptureAvatarAction(int frame, Action action)
        {
            if (Director.PlayState == State.PlayState.Recording && !actionFrames.ContainsKey(frame))
            {
                actionFrames.Add(frame, action);
            }
        }

        public void CloneAvatar()
        {
            GameObject clonedAvatarObject = GameObject.Instantiate(playerAvatar.gameObject);
            clonedAvatar = clonedAvatarObject.GetComponent<SLZ.VRMK.Avatar>();

            clonedRigBones = GetAvatarBones(clonedAvatar);
            GameObject.Destroy(clonedAvatar.GetComponent<LODGroup>());

            actorName = $"Actor - {Constants.rigManager.AvatarCrate.Crate.Title}";
            clonedAvatar.name = actorName;
            ShowHairMeshes(clonedAvatar);

            GameObject.FindObjectOfType<PullCordDevice>().PlayAvatarParticleEffects();

            Events.OnActorCasted?.Invoke(this);
        }

        private void ShowHairMeshes(SLZ.VRMK.Avatar avatar)
        {
            foreach (var mesh in avatar.hairMeshes)
            {
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
            }
        }

        private List<ObjectFrame> CaptureBoneFrames(Transform[] boneList)
        {
            List<ObjectFrame> frames = new List<ObjectFrame>();

            for (int i = 0; i < boneList.Length; i++)
            {
                frames.Add(new ObjectFrame(boneList[i]));
            }

            return frames;
        }

        private Transform[] GetAvatarBones(SLZ.VRMK.Avatar avatar)
        {
            Transform[] bones = new Transform[(int)HumanBodyBones.LastBone];

            for (int i = 0; i < bones.Length; i++)
            {
                var currentBone = (HumanBodyBones)i;

                if (avatar.animator.GetBoneTransform(currentBone) == null)
                {
                    continue;
                }
                else
                {
                    var boneTransform = avatar.animator.GetBoneTransform(currentBone);
                    bones[i] = boneTransform;
                }
            }

            return bones;
        }
    }
}
