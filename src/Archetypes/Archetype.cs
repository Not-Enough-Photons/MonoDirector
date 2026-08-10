using UnityEngine;
using NEP.MonoDirector.Keyframes;

namespace NEP.MonoDirector.Archetypes;

public abstract class Archetype
{
    protected Archetype()
    {
        m_positionTracks = new List<KeyframeTrack<Vector3>>();
        m_rotationTracks = new List<KeyframeTrack<Quaternion>>();
        //m_eventTrack = new KeyframeTrack<EventPacket>();
    }
	
    public bool Armed => m_armed;
    public bool Visible => m_visible;

    protected List<KeyframeTrack<Vector3>> m_positionTracks;
    protected List<KeyframeTrack<Quaternion>> m_rotationTracks;
    //private KeyframeTrack<EventPacket> m_eventTrack;

    //private Queue<EventPacket> m_queuedEvents;
	
    protected bool m_armed;
    protected bool m_visible;

    public void AddPositionTrack(string name)
    {
        m_positionTracks.Add(new KeyframeTrack<Vector3>(name));
    }
	
    public void AddRotationTrack(string name)
    {
        m_rotationTracks.Add(new KeyframeTrack<Quaternion>(name));
    }
	
    public abstract void Initialize();
    public abstract void Destroy();
    public abstract void SceneBegin();
    public abstract void SceneEnd();
    public abstract void Act(float time);
    public abstract void Capture(float time);

    /// Arm this archetype for recording.
    public void Arm() => m_armed = true;

    /// Disarm this archetype for recording.
    public void Disarm() => m_armed = false;

    public void SetVisible(bool visible) => m_visible = visible;

    /*public void EnqueueEvent(EventPacket e)
    {
        m_queuedEvents.Enqueue(e);
    }

    private void ProcessQueuedEvents(float time)
    {
        while (!m_queuedEvents.Empty())
        {
            EventPacket packet = m_queuedEvents.Dequeue();
            m_eventTrack.Add(time, packet);
        }
    }*/
}