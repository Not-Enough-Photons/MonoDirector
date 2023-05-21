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

        protected override void Awake()
        {
            base.Awake();

            propFrames = new List<ObjectFrame>();
            actionFrames = new List<ActionFrame>();
        }

        public override void OnSceneBegin()
        {
            base.OnSceneBegin();

            foreach(ActionFrame actionFrame in actionFrames)
            {
                actionFrame.Reset();
            }
        }

        public void SetBreakableObject(ObjectDestructable destructable)
        {
            this.breakableProp = destructable;
        }

        public void DestructionEvent()
        {
            breakableProp._isDead = false;
            breakableProp.TakeDamage(Vector3.zero, 100f, true);
            gameObject.SetActive(false);
        }
    }
}
