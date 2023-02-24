using System.Collections.Generic;

using UnityEngine;

using NEP.MonoDirector.Data;
using SLZ.Combat;

namespace NEP.MonoDirector.Actors
{
    public class ActorNPC
    {
        public ActorNPC(Transform root)
        {
            this.root = root;

            var meshContainer = root.GetComponent<VisualDamageController>().Renderers;

            meshes = new List<Renderer>();

            foreach(var mesh in meshContainer)
            {
                meshes.Add(mesh);
            }
        }

        private string actorName;
        private int actorId;
        private Transform root;
        private Dictionary<int, FrameGroup> actorFrames;

        private List<Renderer> meshes;
        private Transform[] npcBones;
        private Transform[] clonedNPCBones;

        private int recordedTicks;

        /// <summary>
        /// Updates the actor's pose on this recorded frame.
        /// </summary>
        /// <param name="currentFrame">The frame to act, or to display the pose on that frame.</param>
        public void Act(int currentFrame)
        {
            // We've reached past our recorded ticks, don't proceed further!
            if (currentFrame >= recordedTicks)
            {
                return;
            }

            if (!actorFrames.ContainsKey(currentFrame))
            {
                return;
            }

            var actorFrame = actorFrames[currentFrame];

            for (int i = 0; i < actorFrame.transformFrames.Count; i++)
            {
                var boneFrame = actorFrame.transformFrames[i];
                boneFrame.transform = clonedNPCBones[i];

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
        public void CaptureActorFrame()
        {
            actorFrames.Add(recordedTicks++, new FrameGroup(CaptureBoneFrames(meshes.ToArray())));
        }

        public void CloneNPC()
        {

        }

        public void ShowActor(bool show)
        {

        }

        public void Delete()
        {

        }

        private List<ObjectFrame> CaptureBoneFrames(Renderer[] boneList)
        {
            List<ObjectFrame> frames = new List<ObjectFrame>();

            for (int i = 0; i < boneList.Length; i++)
            {
                frames.Add(new ObjectFrame(boneList[i].transform));
            }

            return frames;
        }
    }
}
