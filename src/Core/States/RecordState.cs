using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Content;
using UnityEngine;

namespace NEP.MonoDirector.Core.States;

public sealed class RecordState : State
{
    private float m_timeSinceLastUpdate;
    private float m_frameTimer;
    
    public override void Start()
    {
        Playhead.Reset();
        Playhead.UseLimit(false);
        Playhead.UseLoop(false);

        // Preliminary Fusion support, I think. Obviously not optimized to reuse old avatars,
        // so a ton of memory will be wasted with each new recording.
        // TODO: Replace this system with something more flexible?
        //       Perhaps potential actors can be selected by the Fusion lobby host?
        //       Also, add a system for actor avatar pools to save on memory.
        foreach (var rig in Director.TrackedRigs)
            ActorBuilder.CreateActor(rig.avatar);
        
        foreach (var archetype in Caster.Archetypes)
        {
            if (!archetype.Armed)
                archetype.SceneBegin();
        }
    }

    public override void Update()
    {
        UpdatePlayhead();
        
        foreach (var archetype in Caster.Archetypes)
        {
            if (!archetype.Armed)
                archetype.Act(Playhead.Time);
            else
                archetype.Capture(Playhead.Time);
        }
    }

    public override void Stop()
    {
        Director.Scene.SetDuration(Playhead.Duration);
        
        foreach (var archetype in Caster.Archetypes)
        {
            if (!archetype.Armed)
                archetype.SceneEnd();
            else
            {
                archetype.Disarm();
                
                if (archetype is Actor actor)
                    ActorBuilder.CreateClone(actor);
            }
        }

        Director.PlaySound(BundleLoader.BeepClip);
    }

    private void UpdatePlayhead()
    {
        if (Settings.IgnoreSlowmo)
            m_timeSinceLastUpdate += Time.deltaTime;
        else
            m_timeSinceLastUpdate += Time.unscaledDeltaTime;

        if (Settings.TemporalScaling)
            m_frameTimer += Time.unscaledDeltaTime;
        else
            m_frameTimer += Time.deltaTime;

        if (m_frameTimer > Playhead.PerTick)
        {
            Playhead.Advance(m_timeSinceLastUpdate);
            
            m_frameTimer = 0f;
            m_timeSinceLastUpdate = 0f;
        }
    }
}