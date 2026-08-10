using NEP.MonoDirector.Archetypes;

namespace NEP.MonoDirector.Core;

public static class Caster
{
    public static event Action<Actor> OnActorAdded;
    public static event Action<Actor> OnActorRemoved;
    public static event Action<Actor> OnActorRecasted;
    public static event Action<Actor> OnActorSelected;
    public static event Action<Actor> OnActorDeselected;

    public static event Action<Prop> OnPropAdded;
    public static event Action<Prop> OnPropRemoved;
    public static event Action<Prop> OnPropSelected;
    public static event Action<Prop> OnPropDeselected;

    public static IReadOnlyList<Actor> SelectedActors => m_selectedActors.AsReadOnly();
    public static IReadOnlyList<Prop> SelectedProps => m_selectedProps.AsReadOnly();

    public static IReadOnlyList<Actor> Actors => m_actors.AsReadOnly();
    public static IReadOnlyList<Prop> Props => m_props.AsReadOnly();

    private static List<Actor> m_selectedActors;
    private static List<Prop> m_selectedProps;

    private static List<Actor> m_actors;
    private static List<Prop> m_props;

    internal static void Initialize()
    {
        m_actors = new List<Actor>();
        m_props = new List<Prop>();
        m_selectedActors = new List<Actor>();
        m_selectedProps = new List<Prop>();
    }

    public static void AddActor(Actor actor)
    {
        m_actors.Add(actor);
        OnActorAdded?.Invoke(actor);
    }

    public static void RemoveActor(Actor actor)
    {
        m_actors.Remove(actor);
        OnActorRemoved?.Invoke(actor);
    }
    
    public static void RecastActor(Actor actor)
    {
        OnActorRecasted?.Invoke(actor);
    }
    
    public static void SelectActor(Actor actor)
    {
        m_selectedActors.Add(actor);
        OnActorSelected?.Invoke(actor);
    }
    
    public static void DeselectActor(Actor actor)
    {
        m_selectedActors.Remove(actor);
        OnActorDeselected?.Invoke(actor);
    }

    public static void AddProp(Prop prop)
    {
        m_props.Add(prop);
        OnPropAdded?.Invoke(prop);
    }
    
    public static void RemoveProp(Prop prop)
    {
        m_props.Remove(prop);
        OnPropRemoved?.Invoke(prop);
    }
}
