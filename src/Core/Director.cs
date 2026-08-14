using Il2CppSLZ.Marrow;
using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Core.States;
using NEP.MonoDirector.Visuals;
using UnityEngine;

namespace NEP.MonoDirector.Core;

public static class Director
{
    public static event Action<Film> OnFilmChanged;

    public static event Action<Scene> OnSceneAdded;
    public static event Action<Scene> OnSceneChanged;
    public static event Action<Scene> OnSceneRemoved;

    public static bool Playing => m_playing;
    
    public static IReadOnlyList<RigManager> TrackedRigs => m_trackedRigs.AsReadOnly();

    public static Film Film => m_film;
    public static Scene Scene => m_scene;
    
    private static PlayState PlayState;
    private static RecordState RecordState;
    private static CountdownState CountdownState;

    private static List<RigManager> m_trackedRigs;

    private static Film m_film;
    private static Scene m_scene;
    
    private static State m_currentState;

    private static bool m_playing;

    public static void Initialize()
    {
        m_trackedRigs = new List<RigManager>();
        
        m_playing = false;

        PlayState = new PlayState();
        RecordState = new RecordState();
        CountdownState = new CountdownState();
        
        // Always track the local player.
        TrackRig(BoneLib.Player.RigManager);
        
        Playhead.Initialize();
        Caster.Initialize();
        
        ActorBuilder.Initialize();
        PropBuilder.Initialize();

        m_film = new Film();
        m_scene = new Scene();
        
        AddScene(m_scene);
    }
    
    public static void Play()
    {
        m_playing = true;

        if (m_currentState != PlayState)
        {
            CountdownState.SetCounts(Settings.CountdownSeconds);
            CountdownState.SetNextState(PlayState);
            SwitchTo(CountdownState);
        }
    }

    public static void Record()
    {
        m_playing = true;

        if (m_currentState != RecordState)
        {
            CountdownState.SetCounts(Settings.CountdownSeconds);
            CountdownState.SetNextState(RecordState);
            SwitchTo(CountdownState);
        }
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
        
        m_currentState?.Update();
    }

    public static void SetFilm(Film film)
    {
        if (film is null)
            throw new NullReferenceException("Film was null!");
        
        m_film = film;
        SetScene(m_film.Scenes[0]);
        
        OnFilmChanged?.Invoke(m_film);
    }

    public static void AddScene(Scene scene)
    {
        m_film.AddScene(scene);
        OnSceneAdded?.Invoke(scene);
    }

    public static void RemoveScene(Scene scene)
    {
        m_film.RemoveScene(scene);
        OnSceneRemoved?.Invoke(scene);
    }

    public static void SetScene(Scene scene)
    {
        if (scene is null)
            throw new NullReferenceException("Scene was null!");
        
        var archetypes = Caster.Archetypes.ToList();

        foreach (var archetype in archetypes)
        {
            archetype.Hide();
            
            if (archetype is Actor actor)
                Caster.RemoveActor(actor);
            else if (archetype is Prop prop)
                Caster.RemoveProp(prop);
        }
        
        m_scene = scene;
        
        foreach (var archetype in m_scene.Archetypes)
        {
            archetype.Show();
            
            if (archetype is Actor actor)
                Caster.AddActor(actor);
            else if (archetype is Prop prop)
                Caster.AddProp(prop);
        }
        
        Playhead.SetDuration(m_scene.Duration);
        
        OnSceneChanged?.Invoke(m_scene);
    }
    
    public static void ClearScene()
    {
        Stop();

        var archetypes = Caster.Archetypes.ToList();

        foreach (var archetype in archetypes)
        {
            if (archetype is Actor actor)
                ActorBuilder.DestroyActor(actor);
            else if (archetype is Prop prop)
                PropBuilder.DestroyProp(prop.Entity);
        }
    }

    public static void SwitchTo(State state)
    {
        if (state == null)
            throw new NullReferenceException("The state passed in to Director.SwitchTo was null!");

        m_currentState?.Stop();
        m_currentState = state;
        m_currentState.Start();
    }

    public static void TrackRig(RigManager rig)
    {
        m_trackedRigs.Add(rig);
    }

    public static void UntrackRig(RigManager rig)
    {
        m_trackedRigs.Remove(rig);
    }

    public static void PlaySound(AudioClip clip)
    {
        BoneLib.Audio.Play2DOneShot(clip, BoneLib.Audio.UI, 1f, 1f);
    }
}