using NEP.MonoDirector.Content;
using NEP.MonoDirector.Visuals;
using UnityEngine;

namespace NEP.MonoDirector.Core.States;

public sealed class PlayState : State
{
    public override void Start()
    {
        Playhead.Reset();
        Playhead.UseLimit(true);
        Playhead.UseLoop(false);
        
        VisualManager.HideAll();

        foreach (var archetype in Caster.Archetypes)
        {
            if (!archetype.Armed)
                archetype.SceneBegin();
        }
    }

    public override void Update()
    {
        if (!Playhead.Finished)
            Playhead.Advance(Time.deltaTime);
        else
        {
            if (Playhead.Looping)
                Start();
            else
                Stop();
        }
        
        foreach (var archetype in Caster.Archetypes)
        {
            if (!archetype.Armed)
                archetype.Act(Playhead.Time);
        }
    }

    public override void Stop()
    {
        foreach (var archetype in Caster.Archetypes)
        {
            if (!archetype.Armed)
                archetype.SceneEnd();
        }
        
        VisualManager.ShowAll();
        
        Director.PlaySound(BundleLoader.BeepClip);
    }
}