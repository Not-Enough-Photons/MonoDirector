using System;
using System.Collections.Generic;

using NEP.MonoDirector.Data;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    public class Actor
    {
        public Actor()
        {
            objectFrames = new Dictionary<int, ObjectFrame>();
            actionFrames = new Dictionary<int, Action>();
        }

        public string ActorName { get => actorName; }
        public int ActorId { get => actorId; }

        protected string actorName;
        protected int actorId;

        protected Transform transform;
        protected Dictionary<int, ObjectFrame> objectFrames;
        protected Dictionary<int, Action> actionFrames;

        protected int stateTick;
        protected int recordedTicks;

        /// <summary>
        /// Updates the actor's pose on this recorded frame.
        /// </summary>
        /// <param name="currentFrame">The frame to act, or to display the pose on that frame.</param>
        public virtual void Act(int currentFrame)
        {
            if (!CanAct(currentFrame))
            {
                return;
            }

            var objectFrame = objectFrames[currentFrame];

            transform.position = objectFrame.position;
            transform.rotation = objectFrame.rotation;

            if (actionFrames.ContainsKey(currentFrame))
            {
                actionFrames[currentFrame]?.Invoke();
            }
        }

        public bool CanAct(int frame)
        {
            // We've reached past our recorded ticks, don't proceed further!
            if (frame >= recordedTicks)
            {
                return false;
            }

            if (!objectFrames.ContainsKey(frame))
            {
                return false;
            }

            return true;
        }

        public virtual void RecordFrame()
        {
            objectFrames.Add(recordedTicks++, new ObjectFrame(transform));
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
        }

        public virtual void Delete()
        {
            objectFrames.Clear();
            actionFrames.Clear();
            GameObject.Destroy(transform.gameObject);
            transform = null;
        }
    }
}