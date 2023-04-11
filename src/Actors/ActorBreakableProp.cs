using SLZ.Props;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorBreakableProp : ActorProp
    {
        public ActorBreakableProp(IntPtr ptr) : base(ptr) { }

        public ObjectDestructable breakableProp;

        private Dictionary<int, Action> eventDictionary;

        protected override void Awake()
        {
            base.Awake();

            eventDictionary = new Dictionary<int, Action>();
        }

        public void SetBreakableObject(ObjectDestructable destructable)
        {
            this.breakableProp = destructable;
        }

        public override void Act()
        {
            base.Act();
        }

        public void RecordDestructionEvent(int timeStamp, Action action)
        {
            if (!eventDictionary.ContainsKey(timeStamp))
            {
                eventDictionary.Add(timeStamp, action);
            }
        }

        public void DestructionEvent()
        {
            breakableProp._isDead = false;
            breakableProp.TakeDamage(Vector3.zero, 100f, true);
            gameObject.SetActive(false);
        }
    }
}
