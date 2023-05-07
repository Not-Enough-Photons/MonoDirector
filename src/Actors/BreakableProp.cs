using SLZ.Props;
using System;
using System.Collections.Generic;
using UnityEngine;

using NEP.MonoDirector.Data;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class BreakableProp : Prop
    {
        public BreakableProp(IntPtr ptr) : base(ptr) { }

        public ObjectDestructable breakableProp;

        private List<ActionFrame> actionFrames;

        protected override void Awake()
        {
            base.Awake();

            actionFrames = new List<ActionFrame>();
        }

        public void SetBreakableObject(ObjectDestructable destructable)
        {
            this.breakableProp = destructable;
        }

        public override void Act()
        {
            base.Act();

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
        }

        public override void OnSceneBegin()
        {
            base.OnSceneBegin();

            foreach (ActionFrame actionFrame in actionFrames)
            {
                actionFrame.Reset();
            }
        }

        public void RecordDestructionEvent(float timeStamp, Action action)
        {
            actionFrames.Add(new ActionFrame(action, timeStamp));
        }

        public void DestructionEvent()
        {
            breakableProp._isDead = false;
            breakableProp.TakeDamage(Vector3.zero, 100f, true);
            gameObject.SetActive(false);
        }
    }
}
