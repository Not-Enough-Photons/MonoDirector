﻿using System;
using System.Collections.Generic;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

using SLZ.Props;
using SLZ.Vehicle;
using UnityEngine;

using Avatar = SLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Actors
{
    public class Actor : Trackable
    {
        public Actor(SLZ.VRMK.Avatar avatar) : base()
        {
            playerAvatar = avatar;
            avatarBones = GetAvatarBones(playerAvatar);
            avatarFrames = new List<FrameGroup>();

            GameObject micObject = new GameObject("Actor Microphone");
            microphone = micObject.AddComponent<ActorMic>();

            tempFrames = new ObjectFrame[55];
        }

        public Avatar PlayerAvatar { get => playerAvatar; }
        public Avatar ClonedAvatar { get => clonedAvatar; }
        public Transform[] AvatarBones { get => avatarBones; }

        public ActorMic Microphone { get => microphone; }

        public bool Seated { get => activeSeat != null; }

        protected List<FrameGroup> avatarFrames;

        private ActorBody body;
        private ActorMic microphone;

        private SLZ.Vehicle.Seat activeSeat;

        private Avatar playerAvatar;
        private Avatar clonedAvatar;

        private ObjectFrame[] tempFrames;

        private Transform[] avatarBones;
        private Transform[] clonedRigBones;

        private FrameGroup previousFrame;
        private FrameGroup nextFrame;

        private Transform lastPelvisParent;

        public override void OnSceneBegin()
        {
            base.OnSceneBegin();

            for (int i = 0; i < 55; i++)
            {
                var bone = clonedRigBones[i];

                if (bone == null)
                {
                    continue;
                }

                bone.position = avatarFrames[0].transformFrames[i].position;
                bone.rotation = avatarFrames[0].transformFrames[i].rotation;
            }
        }

        public override void Act()
        {
            previousFrame = new FrameGroup();
            nextFrame = new FrameGroup();

            foreach (var frame in avatarFrames)
            {
                previousFrame = nextFrame;
                nextFrame = frame;

                if (frame.frameTime > Playback.instance.PlaybackTime)
                {
                    break;
                }
            }

            float gap = nextFrame.frameTime - previousFrame.frameTime;
            float head = Playback.instance.PlaybackTime - previousFrame.frameTime;

            float delta = head / gap;

            ObjectFrame[] previousTransformFrames = previousFrame.transformFrames;
            ObjectFrame[] nextTransformFrames = nextFrame.transformFrames;

            for (int i = 0; i < 55; i++)
            {
                if(i == (int)HumanBodyBones.Jaw)
                {
                    continue;
                }

                if (previousTransformFrames == null)
                {
                    continue;
                }

                Vector3 previousPosition = previousTransformFrames[i].position;
                Vector3 nextPosition = nextTransformFrames[i].position;

                Quaternion previousRotation = previousTransformFrames[i].rotation;
                Quaternion nextRotation = nextTransformFrames[i].rotation;

                clonedRigBones[i].position = Vector3.Lerp(previousPosition, nextPosition, delta);
                clonedRigBones[i].rotation = Quaternion.Slerp(previousRotation, nextRotation, delta);
            }

            foreach (ActionFrame actionFrame in actionFrames)
            {
                if (Playback.instance.PlaybackTime < actionFrame.timestamp)
                {
                    continue;
                }
                else
                {
                    actionFrame.Run();
                }
            }

            microphone?.Playback();
            microphone?.UpdateJaw();
        }

        /// <summary>
        /// Records the actor's bones, positons, and rotations for this frame.
        /// </summary>
        /// <param name="index">The frame to record the bones.</param>
        public override void RecordFrame()
        {
            FrameGroup frameGroup = new FrameGroup();
            CaptureBoneFrames(avatarBones);
            frameGroup.SetFrames(tempFrames, Recorder.instance.RecordingTime);
            avatarFrames.Add(frameGroup);
        }

        public void CloneAvatar()
        {
            GameObject clonedAvatarObject = GameObject.Instantiate(playerAvatar.gameObject);
            clonedAvatar = clonedAvatarObject.GetComponent<SLZ.VRMK.Avatar>();

            clonedAvatar.gameObject.SetActive(true);

            body = new ActorBody(this, Constants.rigManager.physicsRig);

            // stops position overrides, if there are any
            clonedAvatar.GetComponent<Animator>().enabled = false;

            clonedRigBones = GetAvatarBones(clonedAvatar);

            GameObject.Destroy(clonedAvatar.GetComponent<LODGroup>());

            actorName = $"Actor - {Constants.rigManager.AvatarCrate.Crate.Title}";
            clonedAvatar.name = actorName;
            ShowHairMeshes(clonedAvatar);

            GameObject.FindObjectOfType<PullCordDevice>().PlayAvatarParticleEffects();

            microphone.SetAvatar(clonedAvatar);

            clonedAvatar.gameObject.SetActive(true);

            Events.OnActorCasted?.Invoke(this);
        }

        public void SwitchToActor(Actor actor)
        {
            Main.Logger.Msg("SwitchToAvatar");
            clonedAvatar.gameObject.SetActive(false);
            actor.clonedAvatar.gameObject.SetActive(true);
        }

        public void Delete()
        {
            body.Delete();
            GameObject.Destroy(clonedAvatar.gameObject);
            GameObject.Destroy(microphone.gameObject);
            microphone = null;
            avatarFrames.Clear();
        }

        public void ParentToSeat(SLZ.Vehicle.Seat seat)
        {
            activeSeat = seat;

            Transform pelvis = clonedAvatar.animator.GetBoneTransform(HumanBodyBones.Hips);

            lastPelvisParent = pelvis.GetParent();

            Vector3 seatOffset = new Vector3(seat._buttOffset.x, Mathf.Abs(seat._buttOffset.y) * clonedAvatar.heightPercent, seat._buttOffset.z);

            pelvis.SetParent(seat.transform);

            pelvis.position = seat.buttTargetInWorld;
            pelvis.localPosition = seatOffset;
        }

        public void UnparentSeat()
        {
            activeSeat = null;
            Transform pelvis = clonedAvatar.animator.GetBoneTransform(HumanBodyBones.Hips);
            pelvis.SetParent(lastPelvisParent);
        }

        private void ShowHairMeshes(SLZ.VRMK.Avatar avatar)
        {
            foreach (var mesh in avatar.hairMeshes)
            {
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
            }
        }

        private void CaptureBoneFrames(Transform[] boneList)
        {
            for (int i = 0; i < boneList.Length; i++)
            {
                Vector3 bonePosition = boneList[i].position;
                Quaternion boneRotation = boneList[i].rotation;

                ObjectFrame frame = new ObjectFrame(bonePosition, boneRotation);
                tempFrames[i] = frame;
            }
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
