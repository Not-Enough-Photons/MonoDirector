using NEP.MonoDirector.Core;

using UnityEngine;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Archetypes;

public static class ActorBuilder
{
    public static Dictionary<GameObject, Actor> AvatarTable;

    public static void Initialize()
    {
        AvatarTable = new Dictionary<GameObject, Actor>();
    }
    
    public static void CreateActor(MarrowAvatar avatar)
    {
        if (avatar is null)
            throw new NullReferenceException("Null avatar passed into ActorBuilder.CreateActor! Something went terribly, terribly wrong.");

        Actor actor = new Actor(avatar);
        actor.Arm();
        
        AvatarTable.Add(avatar.gameObject, actor);
        
        Caster.AddActor(actor);
        Director.Scene.AddArchetype(actor);
    }

    public static void DestroyActor(Actor actor)
    {
        AvatarTable.Remove(actor.Avatar.gameObject);
        actor.Destroy();
        Caster.RemoveActor(actor);
        Director.Scene.RemoveArchetype(actor);
    }

    public static void CreateClone(Actor actor)
    {
        var clone = GameObject.Instantiate(actor.Avatar.gameObject).GetComponent<MarrowAvatar>();
        actor.SetAvatar(clone);
    }
}