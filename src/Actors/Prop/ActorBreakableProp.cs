using SLZ.Props;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    public class ActorBreakableProp : ActorProp
    {
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

        public override void Act(int currentTick)
        {
            base.Act(currentTick);

            if (eventDictionary.ContainsKey(currentTick))
            {
                eventDictionary[currentTick]?.Invoke();
            }
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
