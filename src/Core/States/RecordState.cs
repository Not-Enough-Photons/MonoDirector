using NEP.MonoDirector.Archetypes;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Core.States;

public sealed class RecordState : State
{
    public override void Start()
    {
        Playhead.Reset();
        Playhead.UseLimit(false);
        
        ActorBuilder.CreateActor(BoneLib.Player.Avatar);
        
        foreach (var actor in Caster.Actors)
        {
            if (!actor.Armed)
                actor.SceneBegin();
        }

        foreach (var prop in Caster.Props)
        {
            if (!prop.Armed)
                prop.SceneBegin();
        }
    }

    public override void Update(float time)
    {
        foreach (var actor in Caster.Actors)
        {
            if (!actor.Armed)
                actor.Act(time);
            else
                actor.Capture(time);
        }

        foreach (var prop in Caster.Props)
        {
            if (!prop.Armed)
                prop.Act(time);
            else
                prop.Capture(time);
        }
    }

    public override void Stop()
    {
        foreach (var actor in Caster.Actors)
        {
            if (!actor.Armed)
                actor.SceneEnd();
            else
            {
                actor.Disarm();
                ActorBuilder.CreateClone(actor);
            }
        }

        foreach (var prop in Caster.Props)
        {
            if (!prop.Armed)
                prop.SceneEnd();
            else
                prop.Disarm();
        }
    }
}