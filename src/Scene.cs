using System.Text;

using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Data;
using UnityEngine;

namespace NEP.MonoDirector.Core;

public sealed class Scene : IBinaryData
{
    public Scene()
    {
        m_actors = new List<Actor>();
        m_props = new List<Prop>();
        m_entities = new List<Entity>();
        m_name = "Scene";
    }

    public Scene(string name)
    {
        m_actors = new List<Actor>();
        m_props = new List<Prop>();
        m_entities = new List<Entity>();
        m_name = name;
    }

    public IReadOnlyList<Actor> Actors => m_actors.AsReadOnly();
    public IReadOnlyList<Prop> Props => m_props.AsReadOnly();
    public IReadOnlyList<Entity> Entities => m_entities.AsReadOnly();
    public string Name => m_name;
    public float Duration => m_duration;
    public int SceneIndex => m_sceneIndex;

    private List<Actor> m_actors;
    private List<Prop> m_props;
    private List<Entity> m_entities;
    private string m_name;
    private float m_duration;
    private int m_sceneIndex;

    public static void Swap(ref Scene left, ref Scene right)
    {
        Scene temp = left;
        left = right;
        right = temp;
    }

    public void AddActor(Actor actor)
    {
        m_actors.Add(actor);
    }

    public void AddActors(List<Actor> actors)
    {
        m_actors.AddRange(actors);
    }

    public void RemoveActor(Actor actor)
    {
        m_actors.Remove(actor);
    }

    public void RemoveActors(List<Actor> actors)
    {

    }

    public void AddProp(Prop prop)
    {
        m_props.Add(prop);
    }

    public void AddProps(List<Prop> props)
    {
        m_props.AddRange(props);
    }

    public void RemoveProp(Prop prop)
    {
        m_props.Remove(prop);
    }

    public void RemoveProps(List<Prop> props)
    {

    }
    
    public void AddEntity(Entity entity)
    {
        m_entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        m_entities.Remove(entity);
    }

    public void SetDuration(float duration)
    {
        m_duration = duration;
    }

    public void SetIndex(int index)
    {
        m_sceneIndex = index;
    }

    public byte[] ToBinary()
    {
        // Header Information:
        // Version: u16
        // Scene Name Length: u32
        // Scene Name: UTF-8 String
        // Duration: f32
        // Number of Actors: u32
        // Number of Props: u32
        // Number of Entities: u32

        using MemoryStream stream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(stream);
        
        writer.Write(BitConverter.GetBytes((ushort)0x0));
        writer.Write(m_name.Length);
        writer.Write(Encoding.UTF8.GetBytes(m_name));
        writer.Write(BitConverter.GetBytes(m_duration));
        writer.Write(BitConverter.GetBytes(m_actors.Count));
        writer.Write(BitConverter.GetBytes(m_props.Count));
        writer.Write(BitConverter.GetBytes(m_entities.Count));
        
        foreach (var actor in m_actors)
            writer.Write(actor.ToBinary());
        
        foreach (var prop in m_props)
            writer.Write(prop.ToBinary());

        foreach (var entity in m_entities)
        {
            switch (entity.EntityType)
            {
                case Entity.Type.Light:
                    LightEntity light = (LightEntity)entity;
                    writer.Write(light.ToBinary());
                    break;
                case Entity.Type.Sound:
                    SoundEntity sound = (SoundEntity)entity;
                    writer.Write(sound.ToBinary());
                    break;
                case Entity.Type.None:
                case Entity.Type.Unknown:
                default: 
                    break;
            }
        }

        return stream.ToArray();
    }

    public void FromBinary(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        ushort version = reader.ReadUInt16();

        if (version == 0x0)
        {
            int sceneNameLength = reader.ReadInt32();
            byte[] sceneNameBytes = reader.ReadBytes(sceneNameLength);
            string sceneName = Encoding.UTF8.GetString(sceneNameBytes);
            float duration = reader.ReadSingle();
            int actorCount = reader.ReadInt32();
            int propCount = reader.ReadInt32();
            int entityCount = reader.ReadInt32();
            
            m_name = sceneName;
            m_duration = duration;

            m_actors = new List<Actor>(actorCount);
            m_props = new List<Prop>(propCount);
            m_entities = new List<Entity>(entityCount);

            for (int i = 0; i < actorCount; i++)
            {
                Actor actor = new Actor();
                actor.FromBinary(stream);
                m_actors.Add(actor);
            }

            for (int i = 0; i < propCount; i++)
            {
                Prop.Type type = (Prop.Type)reader.ReadByte();

                if (type == Prop.Type.Generic)
                {
                    Prop prop = new Prop();
                    prop.FromBinary(stream);
                    m_props.Add(prop);
                }
                else if (type == Prop.Type.Gun)
                {
                    GunProp prop = new GunProp();
                    prop.FromBinary(stream);
                    m_props.Add(prop);
                }
            }

            for (int i = 0; i < entityCount; i++)
            {
                Entity.Type type = (Entity.Type)reader.ReadByte();

                if (type == Entity.Type.Light)
                {
                    LightEntity light = new LightEntity();
                    light.FromBinary(stream);
                    m_entities.Add(light);
                }
                else if (type == Entity.Type.Sound)
                {
                    SoundEntity sound = new SoundEntity();
                    sound.FromBinary(stream);
                    m_entities.Add(sound);
                }
            }
        }
    }

    public uint GetBinaryID() => 0xFFFFFFFF; // idk yet
}
