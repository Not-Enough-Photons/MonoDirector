using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Nodes;
using NEP.MonoDirector.Serialization;
using UnityEngine;

namespace NEP.MonoDirector.Events;

public sealed class LightNodeUpdate : EventPacket, ISerialized
{
    public LightNodeUpdate(LightNode node, byte type, float range, float intensity, Color color)
    {
        // m_archetypeID = node.Archetype.ID;
        m_lightType = type;
        m_lightRange = range;
        m_lightIntensity = intensity;
        m_lightColor = color;
    }
    
    private uint m_archetypeID;

    private byte m_lightType;
    private float m_lightRange;
    private float m_lightIntensity;
    private Color m_lightColor;
    
    public override void Execute()
    {
        throw new NotImplementedException();
    }

    public byte[] Serialize()
    {
        using MemoryStream stream = new MemoryStream();
        using StreamWriter writer = new StreamWriter(stream);
        
        writer.Write(m_lightType);
        writer.Write(m_lightRange);
        writer.Write(m_lightIntensity);
        writer.Write(m_lightColor);

        return stream.GetBuffer();
    }

    public void Deserialize(Stream stream)
    {
        throw new NotImplementedException();
    }
}