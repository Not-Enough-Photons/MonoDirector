using System;
using System.Collections.Generic;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    public class Actor
    {
        public string ActorName { get => actorName; }
        public int ActorId { get => actorId; }

        protected string actorName;
        protected int actorId;

        protected Transform transform;
        protected ObjectFrame[] objectFrames;
        protected Action[] actionFrames;

        protected int stateTick;
        protected int recordedTicks;

        /// <summary>
        /// Updates the actor's pose on this recorded frame.
        /// </summary>
        /// <param name="currentFrame">The frame to act, or to display the pose on that frame.</param>
        public virtual void Act()
        {
            var objectFrame = objectFrames[Playback.instance.PlaybackTick];

            transform.position = objectFrame.position;
            transform.rotation = objectFrame.rotation;
        }

        public virtual bool CanAct(int frame)
        {
            // We've reached past our recorded ticks, don't proceed further!
            if (frame >= recordedTicks)
            {
                return false;
            }

            return true;
        }

        public virtual void RecordFrame()
        {
            List<ObjectFrame> objectFrames = new List<ObjectFrame>();

            ObjectFrame objectFrame = new ObjectFrame(transform);

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