namespace NEP.MonoDirector.Core.States;

public sealed class PlayState : State
{
    public override void Start()
    {
        Playhead.Reset();
        Playhead.UseLimit(true);
        
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
        }

        foreach (var prop in Caster.Props)
        {
            if (!prop.Armed)
                prop.Act(time);
        }
    }

    public override void Stop()
    {
        foreach (var actor in Caster.Actors)
        {
            if (!actor.Armed)
                actor.SceneEnd();
        }

        foreach (var prop in Caster.Props)
        {
            if (!prop.Armed)
                prop.SceneEnd();
        }
    }
}