using System.Collections.Generic;
using NEP.MonoDirector.Actors;

namespace NEP.MonoDirector
{
    public class Matinee
    {
        public int TakeIndex { get; private set; }

        public List<Take> Takes => takes;
        public List<Actor> Actors => actors;
        public List<Prop> Props => props;

        public float Runtime { get; private set; }

        private List<Actor> actors = new List<Actor>();
        private List<Prop> props = new List<Prop>();

        private List<Take> takes = new List<Take>();

        public void AddTake(Take take)
        {
            takes.Add(take);
            TakeIndex++;
        }

        public void RemoveTake(Take take)
        {
            takes.Remove(take);
            TakeIndex--;
        }

        public void AddActor(Actor actor)
        {
            actors.Add(actor);
        }

        public void AddActors(Actor[] castList)
        {
            actors.AddRange(castList);
        }

        public void AddActors(List<Actor> castList)
        {
            AddActors(castList.ToArray());
        }

        public void AddProp(Prop prop)
        {
            props.Add(prop);
        }

        public void AddProps(Prop[] propList)
        {
            props.AddRange(propList);
        }

        public void AddProps(List<Prop> propList)
        {
            AddProps(propList.ToArray());
        }
    }
}