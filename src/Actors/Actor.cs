using System;
using System.Collections.Generic;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

using SLZ.Props;
using SLZ.Vehicle;
using UnityEngine;

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
        }

        public SLZ.VRMK.Avatar PlayerAvatar { get => playerAvatar; }
        public SLZ.VRMK.Avatar ClonedAvatar { get => clonedAvatar; }
        public Transform[] AvatarBones { get => avatarBones; }

        public ActorBody ActorBody { get => actorBody; }
        public ActorMic Microphone { get => microphone; }

        public bool Seated { get => activeSeat != null; }

        protected List<FrameGroup> avatarFrames;

        private ActorBody actorBody;
        private ActorMic microphone;

        private SLZ.Vehicle.Seat activeSeat;

        private SLZ.VRMK.Avatar playerAvatar;
        private SLZ.VRMK.Avatar clonedAvatar;

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

            List<ObjectFrame> previousTransformFrames = previousFrame.transformFrames;
            List<ObjectFrame> nextTransformFrames = nextFrame.transformFrames;

            for (int i = 0; i < 55; i++)
            {
                if(i == (int)HumanBodyBones.Jaw)
                {
                    continue;
                }

                var bone = clonedRigBones[i];

                if (bone == null)
                {
                    continue;
                }

                if (previousTransformFrames == null)
                {
                    continue;
                }

                Vector3 previousBonePosition = previousTransformFrames[i].position;
                Vector3 nextBonePosition = nextTransformFrames[i].position;

                Quaternion previousBoneRotation = previousTransformFrames[i].rotation;
                Quaternion nextBoneRotation = nextTransformFrames[i].rotation;

                // seat hack for now until i code a better way to do actor parenting/unparenting

                if (!Seated)
                {
                    bone.position = Vector3.Lerp(previousBonePosition, nextBonePosition, delta);
                }

                // still want to update rotations 
                bone.rotation = Quaternion.Slerp(previousBoneRotation, nextBoneRotation, delta);
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
            frameGroup.SetFrames(CaptureBoneFrames(avatarBones), Recorder.instance.RecordingTime);
            avatarFrames.Add(frameGroup);
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

            microphone.SetAvatar(clonedAvatar);

            Events.OnActorCasted?.Invoke(this);
        }

        public void Delete()
        {
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

        private List<ObjectFrame> CaptureBoneFrames(Transform[] boneList)
        {
            List<ObjectFrame> frames = new List<ObjectFrame>();

            for (int i = 0; i < boneList.Length; i++)
            {
                ObjectFrame frame = new ObjectFrame(boneList[i]);
                frames.Add(frame);
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
