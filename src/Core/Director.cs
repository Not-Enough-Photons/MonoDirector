using Il2CppSystem.Diagnostics;
using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Core.States;
using UnityEngine;

namespace NEP.MonoDirector.Core;

public static class Director
{
    private static State m_currentState;
    private static float m_frameTimer;
    private static float m_timeSinceLastTick;

    private static bool m_playing;

    public static void Initialize()
    {
        m_playing = false;
        
        Playhead.Initialize();
        Caster.Initialize();
    }
    
    public static void Play()
    {
        m_playing = true;
        
        if (m_currentState is not PlayState)
            SwitchTo(new PlayState());
    }

    public static void Record()
    {
        m_playing = true;
        
        if (m_currentState is not RecordState)
            SwitchTo(new RecordState());
    }

    public static void Pause()
    {
        m_playing = false;
    }

    public static void Stop()
    {
        m_playing = false;
        m_currentState?.Stop();
        m_currentState = null;
    }

    public static void Update()
    {
        if (!m_playing)
            return;
        
        if (Settings.IgnoreSlowmo)
            m_timeSinceLastTick += Time.deltaTime;
        else
            m_timeSinceLastTick += Time.unscaledDeltaTime;

        if (Settings.TemporalScaling)
            m_frameTimer += Time.unscaledDeltaTime;
        else
            m_frameTimer += Time.deltaTime;

        if (m_frameTimer > Playhead.PerTick)
        {
            Playhead.Advance(m_timeSinceLastTick);
            
            m_currentState?.Update(Playhead.Time);
            
            m_frameTimer = 0f;
            m_timeSinceLastTick = 0f;
        }
    }

    public static void ClearScene()
    {
        Stop();
        
        var actors = Caster.Actors;
        var props = Caster.Props;

        foreach (var actor in actors)
            ActorBuilder.DestroyActor(actor);
        
        foreach (var prop in props)
            PropBuilder.DestroyProp(prop.Entity);
    }

    private static void SwitchTo(State state)
    {
        if (state == null)
            throw new NullReferenceException("The state passed in to Director.SwitchTo was null!");

        m_currentState?.Stop();
        m_currentState = state;
        m_currentState.Start();
    }
}