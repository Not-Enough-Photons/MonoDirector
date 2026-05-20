using NEP.MonoDirector.Data;
using NEP.MonoDirector.Core;

using UnityEngine;

namespace NEP.MonoDirector.Archetype;

public abstract class Archetype
{
    protected Archetype()
    {
        m_frames = new List<ObjectFrame>();
        m_actions = new List<ActionFrame>();
    }
    
    protected List<ObjectFrame> m_frames;
    protected List<ActionFrame> m_actions;

    protected ObjectFrame m_previousFrame;
    protected ObjectFrame m_nextFrame;
    
    public abstract void OnSceneBegin();
    public abstract void OnSceneEnd();
    public abstract void Perform();
    public abstract void Record();
    public abstract void RecordAction(Action action);
    public abstract void Delete();
}