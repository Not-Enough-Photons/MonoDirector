using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NEP.MonoDirector.Actors;

namespace NEP.MonoDirector.UI
{
    public class ActorDisplayPage
    {
        public ActorDisplayPage()
        {
            actorEntries = new List<Actor>();
        }

        public List<Actor> actorEntries;
        public int capacity { get; private set; } = 8;
        public int count { get; private set; } = 0;

        private ActorsPage pageInstance;

        public void AddActor(Actor actor)
        {
            if (count + 1 > capacity)
            {
                pageInstance.OnPageFilled(actor);
                return;
            }

            actorEntries.Add(actor);
            count++;
        }

        public void RemoveActor(Actor actor)
        {
            if (count == 0)
            {
                count = 0;
                return;
            }

            actorEntries.Remove(actor);
            count--;
        }

        public void LinkToView(ActorsPage instance)
        {
            pageInstance = instance;
        }
    }
}
