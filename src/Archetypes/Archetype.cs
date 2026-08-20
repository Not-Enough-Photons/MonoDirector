using NEP.MonoDirector.Events;
using NEP.MonoDirector.Keyframes;

using UnityEngine;

namespace NEP.MonoDirector.Archetypes;

public abstract class Archetype
{
    protected Archetype()
    {
        m_positionTracks = new List<KeyframeTrack<Vector3>>();
        m_rotationTracks = new List<KeyframeTrack<Quaternion>>();
        m_eventTrack = new KeyframeTrack<EventPacket>();
        m_queuedEvents = new Queue<EventPacket>();
    }

    public string Barcode => m_barcode;
    public bool Armed => m_armed;
    public bool Visible => m_visible;

    public virtual Type Type => Type.None;

    protected string m_barcode;
    
    protected List<KeyframeTrack<Vector3>> m_positionTracks;
    protected List<KeyframeTrack<Quaternion>> m_rotationTracks;
    protected KeyframeTrack<EventPacket> m_eventTrack;

    private Queue<EventPacket> m_queuedEvents;
	
    protected bool m_armed;
    protected bool m_visible = true;

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
    public abstract byte[] Serialize();
    public abstract void Deserialize(Stream stream);

    /// Arm this archetype for recording.
    public void Arm() => m_armed = true;

    /// Disarm this archetype for recording.
    public void Disarm() => m_armed = false;

    public virtual void Show()
    {
        m_visible = true;
    }

    public virtual void Hide()
    {
        m_visible = false;
    }

    public void SetBarcode(string barcode) => m_barcode = barcode;

    public void EnqueueEvent(EventPacket e)
    {
        m_queuedEvents.Enqueue(e);
    }

    protected void ProcessQueuedEvents(float time)
    {
        while (m_queuedEvents.Count != 0)
        {
            EventPacket packet = m_queuedEvents.Dequeue();
            m_eventTrack.Add(time, packet);
        }
    }

    protected void ActEvents(float time)
    {
        EventPacket packet = GetEventAtTime(time);

        if (packet != null)
        {
            if (!packet.Executed)
                packet.Execute();
            
            packet.MarkExecuted();
        }
    }

    private EventPacket GetEventAtTime(float time)
    {
        foreach (var packet in m_eventTrack.Frames)
        {
            if (packet.Time >= time)
                return packet.Value;
        }

        return null;
    }
}