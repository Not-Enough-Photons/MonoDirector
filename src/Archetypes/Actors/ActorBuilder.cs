using NEP.MonoDirector.Core;
using UnityEngine;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Archetypes;

public static class ActorBuilder
{
    public static void CreateActor(MarrowAvatar avatar)
    {
        if (avatar is null)
            throw new NullReferenceException("Null avatar passed into ActorBuilder.CreateActor! Something went terribly, terribly wrong.");

        Actor actor = new Actor(avatar);
        actor.Arm();
        
        Caster.AddActor(actor);
    }

    public static void DestroyActor(Actor actor)
    {
        actor.Destroy();
        Caster.RemoveActor(actor);
    }

    public static void CreateClone(Actor actor)
    {
        var clone = GameObject.Instantiate(actor.Avatar.gameObject).GetComponent<MarrowAvatar>();
        actor.SetAvatar(clone);
    }
}