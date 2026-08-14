using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Serialization;

namespace NEP.MonoDirector.Core;

public sealed class Scene : ISerialized
{
    public Scene()
    {
        m_name = string.Empty;
        m_duration = 0f;
        m_index = 0;

        m_archetypes = new List<Archetype>();
    }

    public Scene(string name, float duration, int index)
    {
        m_name = name;
        m_duration = duration;
        m_index = index;

        m_archetypes = new List<Archetype>();
    }
    
    public string Name => m_name;
    public float Duration => m_duration;
    public int Index => m_index;

    public IReadOnlyList<Archetype> Archetypes => m_archetypes.AsReadOnly();
    
    private string m_name;
    private float m_duration;
    private int m_index;

    private List<Archetype> m_archetypes;

    public void AddArchetype(Archetype archetype)
    {
        if (archetype is null)
            throw new NullReferenceException("A null archetype was passed into Scene.AddArchetype!");
        
        m_archetypes.Add(archetype);
    }

    public void RemoveArchetype(Archetype archetype)
    {
        m_archetypes.Remove(archetype);
    }
    
    public void SetIndex(int index)
    {
        m_index = index;
    }

    public void SetDuration(float duration)
    {
        m_duration = duration;
    }
    
    public byte[] Serialize()
    {
        throw new NotImplementedException();
    }

    public void Deserialize(Stream stream)
    {
        throw new NotImplementedException();
    }
}