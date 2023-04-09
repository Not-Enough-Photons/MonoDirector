using System;
using System.Collections.Generic;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

using SLZ.VRMK;

using UnityEngine;

using Avatar = SLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Actors
{
    public class ActorPlayer : Actor
    {
        public ActorPlayer(Avatar avatar)
        {
            this.avatar = avatar;

            SkeletonCloner.Clone(ref skeleton, avatar.animator);

            Events.OnStopRecording += OnStopRecording;

            recordedFrames = new List<FrameGroup>();
        }

        protected FrameGroup[] avatarFrames;

        private Avatar avatar;

        private Transform[] skeleton;

        private List<FrameGroup> recordedFrames;

        public void SetSkeleton(Transform[] skeleton)
        {
            this.skeleton = skeleton;
        }

        public override void Act()
        {
            var actorFrame = avatarFrames[Playback.instance.PlaybackTick];

            for (int i = 0; i < actorFrame.transformFrames.Length; i++)
            {
                var frame = actorFrame.transformFrames[i];

                frame.transform.position = frame.position;
                frame.transform.rotation = frame.rotation;
            }
        }

        public void Capture()
        {
            ObjectFrame[] capturedSkeletonFrame = new ObjectFrame[skeleton.Length];

            for(int i = 0; i < skeleton.Length; i++)
            {
                ObjectFrame frame = new ObjectFrame(skeleton[i]);
                frame.SetDelta(Recorder.instance.TimeRecording);
                capturedSkeletonFrame[i] = frame;
            }

            FrameGroup capturedBones = new FrameGroup(capturedSkeletonFrame);
            recordedFrames.Add(capturedBones);
        }

        public override void Delete()
        {
            avatarFrames = null;
            transform = null;
        }

        private void ShowHairMeshes(SLZ.VRMK.Avatar avatar)
        {
            foreach (var mesh in avatar.hairMeshes)
            {
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
            }
        }
        
        private Avatar CloneAvatar()
        {
            GameObject clonedAvatar = GameObject.Instantiate(avatar.gameObject);
            LODGroup lodGroup = clonedAvatar.GetComponent<LODGroup>();
            GameObject.Destroy(lodGroup);
            return clonedAvatar.GetComponent<Avatar>();
        }

        private void OnStopRecording()
        {
            avatarFrames = recordedFrames.ToArray();

            Avatar clonedAvatar = CloneAvatar();

            SkeletonCloner.Clone(ref skeleton, clonedAvatar.animator);

            for(int i = 0; i < avatarFrames.Length; i++)
            {
                for(int j = 0; j < skeleton.Length; j++)
                {
                    var copyFrame = avatarFrames[i].transformFrames[j];
                    SkeletonCloner.SwapBone(ref copyFrame.transform, ref skeleton[j]);
                }
            }

            recordedFrames.Clear();
        }
    }
}
