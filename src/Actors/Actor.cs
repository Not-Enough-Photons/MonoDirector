using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using NEP.MonoDirector.Data;
using UnhollowerRuntimeLib;
using System;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.Actors
{
    public class Actor
    {
        public Actor(SLZ.VRMK.Avatar avatar)
        {
            playerAvatar = avatar;
            actorFrames = new Dictionary<int, TickFrame>();
            actorActionFrames = new Dictionary<int, Action>();
            avatarBones = GetAvatarBones(playerAvatar);
        }

        public string ActorName { get => actorName; }
        public int ActorId { get => actorId; }
        public SLZ.VRMK.Avatar PlayerAvatar { get => playerAvatar; }
        public Transform[] AvatarBones { get => avatarBones; }

        private string actorName;
        private int actorId;
        private SLZ.VRMK.Avatar playerAvatar;
        private SLZ.VRMK.Avatar clonedAvatar;
        private Dictionary<int, TickFrame> actorFrames;
        private Dictionary<int, Action> actorActionFrames;

        private Transform[] avatarBones;
        private Transform[] clonedRigBones;

        protected int stateTick;
        protected int recordedTicks;

        /// <summary>
        /// Updates the actor's pose on this recorded frame.
        /// </summary>
        /// <param name="currentFrame">The frame to act, or to display the pose on that frame.</param>
        public void Act(int currentFrame)
        {
            // We've reached past our recorded ticks, don't proceed further!
            if(currentFrame >= recordedTicks)
            {
                return;
            }

            if (!actorFrames.ContainsKey(currentFrame))
            {
                return;
            }

            var actorFrame = actorFrames[currentFrame];

            for(int i = 0; i < actorFrame.transformFrames.Count; i++)
            {
                var boneFrame = actorFrame.transformFrames[i];
                boneFrame.transform = clonedRigBones[i];

                if(boneFrame.transform == null)
                {
                    continue;
                }

                boneFrame.transform.position = boneFrame.position;
                boneFrame.transform.rotation = boneFrame.rotation;
            }

            if (actorActionFrames.ContainsKey(currentFrame))
            {
                actorActionFrames[currentFrame]?.Invoke();
            }
        }

        /// <summary>
        /// Records the actor's bones, positons, and rotations for this frame.
        /// </summary>
        /// <param name="index">The frame to record the bones.</param>
        public void CaptureAvatarFrame()
        {
            actorFrames.Add(recordedTicks++, new TickFrame(CaptureBoneFrames(avatarBones)));
        }

        public void CaptureAvatarAction(int frame, Action action)
        {
            if(Director.PlayState == State.PlayState.Recording && !actorActionFrames.ContainsKey(frame))
            {
                actorActionFrames.Add(frame, action);
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
            clonedAvatar.gameObject.SetActive(false);

            Events.OnActorCasted?.Invoke(this);
        }

        public void ShowActor(bool show)
        {
            if(clonedAvatar != null)
            {
                if (show)
                {
                    var frames = actorFrames[0];

                    foreach (var frame in frames.transformFrames)
                    {
                        if (frame.transform == null)
                        {
                            continue;
                        }

                        frame.transform.position = frame.position;
                        frame.transform.rotation = frame.rotation;
                    }
                }

                clonedAvatar.gameObject.SetActive(show);
            }
        }

        public void Delete()
        {
            actorFrames.Clear();
            GameObject.Destroy(clonedAvatar.gameObject);
            playerAvatar = null;
        }

        private void ShowHairMeshes(SLZ.VRMK.Avatar avatar)
        {
            foreach(var mesh in avatar.hairMeshes)
            {
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
            }
        }

        private List<ObjectFrame> CaptureBoneFrames(Transform[] boneList)
        {
            List<ObjectFrame> frames = new List<ObjectFrame>();

            for(int i = 0; i < boneList.Length; i++)
            {
                frames.Add(new ObjectFrame(boneList[i]));
            }

            return frames;
        }

        private Transform[] GetAvatarBones(SLZ.VRMK.Avatar avatar)
        {
            Transform[] bones = new Transform[(int)HumanBodyBones.LastBone];

            for(int i = 0; i < bones.Length; i++)
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