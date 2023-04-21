using System;
using System.Collections.Generic;

using NEP.MonoDirector.Data;
using NEP.MonoDirector.Core;

using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    public class Trackable
    {
        public Trackable()
        {
            objectFrames = new List<ObjectFrame>();
            actionFrames = new Dictionary<int, Action>();
        }

        public string ActorName { get => actorName; }
        public int ActorId { get => actorId; }

        protected string actorName;
        protected int actorId;

        protected Transform transform;
        protected List<ObjectFrame> objectFrames;
        protected Dictionary<int, Action> actionFrames;

        protected int stateTick;
        protected int recordedTicks;

        private ObjectFrame previousFrame;
        private ObjectFrame nextFrame;

        /// <summary>
        /// Updates the actor's pose on this recorded frame.
        /// </summary>
        public virtual void Act()
        {
            previousFrame = new ObjectFrame();
            nextFrame = new ObjectFrame();

            foreach (var frame in objectFrames)
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

            transform.position = Vector3.Lerp(previousFrame.position, nextFrame.position, delta);
            transform.rotation = Quaternion.Slerp(previousFrame.rotation, nextFrame.rotation, delta);
        }

        public virtual void RecordFrame()
        {
            ObjectFrame objectFrame = new ObjectFrame()
            {
                transform = transform
            };

            objectFrames.Add(objectFrame);
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
        }

        public virtual void Delete()
        {
            
        }
    }
}