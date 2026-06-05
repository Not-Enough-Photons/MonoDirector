using NEP.MonoDirector.Data;
using NEP.MonoDirector.Core;

using UnityEngine;

namespace NEP.MonoDirector.Archetypes;

/// <summary>
/// An archetype is a concept of a recorded object in MonoDirector.
/// It is like a prototype that contains only frame data.
/// </summary>
public abstract class Archetype
{
    protected Archetype()
    {
        m_frames = new List<FrameGroup>();
        m_actions = new List<ActionFrame>();
    }
    
    public FrameGroup StartFrame => m_frames.First();
    public FrameGroup EndFrame => m_frames.Last();
    
    protected List<FrameGroup> m_frames;
    protected List<ActionFrame> m_actions;

    protected ObjectFrame m_previousFrame;
    protected ObjectFrame m_nextFrame;
    
    public abstract void OnSceneBegin();
    public abstract void OnSceneEnd();
    public abstract void Perform();
    public abstract void Record();
    public abstract void RecordAction(byte type, Action action);
    public abstract void Delete();
}