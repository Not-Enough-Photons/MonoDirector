using System.Text;

using UnityEngine;

using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Pool;

using NEP.MonoDirector.Archetypes.Proxy;
using NEP.MonoDirector.State;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

namespace NEP.MonoDirector.Archetypes;

// NOTE FOR THE FUTURE ABOUT OWNERSHIP:
// Ownership of props is very tricky to implement properly.
// Here are my ideas/criteria for ownership.
// ---------------------------------
// 1. Ungrabbed props are owned by the recording actor by default
// 2. If Actor B grabs it before Actor A, Actor B owns it (and vice versa)
// 3. If at least one recording actor hand is on the prop, they own it
// 4. If multiple actors are grabbing the same prop, whoever grabbed the prop first will still own it
// 5. If nobody is holding/grabbing the prop, nobody owns it
// 6. Ownership cannot be taken away from vehicles; it belongs to whomever propified it first
// 7. Unowned props are physical until grabbed, where they become kinematic
// 8. Ownership transfer of props must override their object frames
// ---------------------------------
// 
// OWNERSHIP THOUGHTS:
// - Mediation of ownership could be done through a class that listens for grabs on props
// - Actors could be refactored to have "actor hands" that also listen for grabs and can help fire events
// - Make this more efficient and not use as much code
// - Maybe not record actions for owning/disowning props

public class Prop : Archetype, IBinaryData
{
    public enum Type
    {
        Generic,
        Gun,
        Vehicle
    }
    
    #if DEBUG
    ~Prop()
    {
        Logging.Msg("GC freed Prop from memory");
    }
    #endif
    
    public Actor Owner => m_owner;
    public PropProxy Proxy => m_proxy;
    public string Barcode => m_barcode;

    public virtual Type PropType => Type.Generic;
    
    protected Actor m_owner;
    protected string m_barcode;
    protected PropProxy m_proxy;
 
    private new FrameGroup m_previousFrame;
    private new FrameGroup m_nextFrame;

    private bool m_isOwned;

    public static bool IsActorProp(MarrowEntity entity)
    {
        if (!entity)
            return false;

        if (entity.gameObject.layer == LayerMask.NameToLayer("EnemyColliders"))
            return false;

        // Prop already exists
        //if (entity.GetComponent<Prop>())
        //    return false;

        return true;
    }

    public static bool EligibleWithType<T>(MarrowEntity entity)
    {
        return entity.GetComponent<T>() != null;
    }

    public void DeleteAllFrames()
    {
        m_frames.Clear();
        m_actions.Clear();
    }

    public void CreateProxy(MarrowEntity entity)
    {
        if (!entity)
            return;

        DestroyProxy();
        
        m_proxy = entity.gameObject.AddComponent<PropProxy>();
        m_proxy.SetProp(this);

        // Spawnable props must have this
        Poolee poolee = m_proxy.GetComponent<Poolee>();

        // Somehow this didn't happen
        if (!poolee)
            return;

        m_barcode = poolee.SpawnableCrate._barcode._id;
    }

    public void DestroyProxy()
    {
        if (!m_proxy)
            return;
        
        GameObject.Destroy(m_proxy);
        m_proxy = null;
    }

    public void SetActor(Actor actor)
    {
        m_owner = actor;
    }

    public override void Delete()
    {
        DestroyProxy();
    }

    public override void OnSceneBegin()
    {
        if (!m_proxy)
            return;
        
        if(m_frames == null)
            return;

        if(m_frames.Count == 0)
            return;

        m_proxy.PoseAtStart();
        m_proxy.SetPhysicsActive(false);

        foreach (var action in m_actions)
            action.Reset();
    }

    public override void OnSceneEnd()
    {
        
    }

    public override void Perform()
    {
        if (!m_proxy)
            return;
        
        m_previousFrame = new FrameGroup();
        m_nextFrame = new FrameGroup();

        for (int i = 0; i < m_frames.Count; i++)
        {
            var frame = m_frames[i];

            m_previousFrame = m_nextFrame;
            m_nextFrame = frame;

            if (frame.FrameTime > Playback.PlaybackTime)
                break;
        }

        m_proxy.Pose(m_previousFrame, m_nextFrame);
        
        foreach(var action in m_actions)
        {
            if (Playback.PlaybackTime < action.timestamp)
            {
                continue;
            }
            else
            {
                action.Run();
            }
        }
    }

    public override void Record()
    {
        if (!m_proxy)
            return;

        FrameGroup group = m_proxy.Record();

        m_frames.Add(group);
    }
    
    public override void RecordAction(byte type, Action action)
    {
        RecordActionAtTime(type, action, Recorder.RecordingTime);
    }

    public void RecordActionAtTime(byte type, Action action, float time = 0f)
    {
        if (Director.PlayState != PlayState.Recording)
            return;
        
        if (!Caster.RecordProps.Contains(this))
            return;

        m_actions.Add(new ActionFrame(type, action, time));
    }

    public uint GetBinaryID()
    {
        throw new NotImplementedException();
    }

    public byte[] ToBinary()
    {
        // The header contains the following data
        //
        // barcode_size: i32
        // barcode : utf-8 string
        // num_frames : u32
        // num_actions : u32

        using MemoryStream stream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(stream);
        
        // writer.Write(BitConverter.GetBytes((ushort)0x0));
        writer.Write((byte)PropType);
        writer.Write(BitConverter.GetBytes(m_barcode.Length));
        writer.Write(Encoding.UTF8.GetBytes(m_barcode));
        writer.Write(BitConverter.GetBytes(m_frames.Count));
        writer.Write(BitConverter.GetBytes(m_actions.Count));
        
        foreach (FrameGroup group in m_frames)
            writer.Write(group.ToBinary());
        
        foreach (ActionFrame action in m_actions)
            writer.Write(action.ToBinary());

        return stream.ToArray();
    }

    public void FromBinary(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        // How long is the string?
        int strlen = reader.ReadInt32();
        byte[] strBytes = reader.ReadBytes(strlen);
        string barcodeID = Encoding.UTF8.GetString(strBytes);

        m_barcode = barcodeID;
            
        // Then deserialize the frames
        int numFrames = reader.ReadInt32();
            
        // Then deserialize the action frames
        int numActionFrames = reader.ReadInt32();

#if DEBUG
        Logging.MsgDebug($"[PROP]: NumFrames: {numFrames}");
#endif

        FrameGroup[] frameGroups = new FrameGroup[numFrames];

        for (int f = 0; f < numFrames; f++)
        {
            frameGroups[f] = new FrameGroup();
            frameGroups[f].FromBinary(stream);
        }

        m_frames = new List<FrameGroup>(frameGroups);

        ActionFrame[] actionFrames = new ActionFrame[numActionFrames];

        for (int f = 0; f < numActionFrames; f++)
        {
            actionFrames[f] = new ActionFrame();
            ActionFrame action = actionFrames[f]; 
            action.FromBinary(stream);
            SetupAction(action);
        }

        m_actions = new List<ActionFrame>(actionFrames);
    }

    protected virtual void SetupAction(ActionFrame action)
    {
        Logging.MsgDebug($"Type {((ActionType)action.type).ToString()} read");
        
        switch ((ActionType)action.type)
        {
            case ActionType.Hide:
                action.action = () => m_proxy.Hide();
                break; 
            case ActionType.Show:
                action.action = () => m_proxy.Show();
                break;
            default:
                break;
        }
    }
}
