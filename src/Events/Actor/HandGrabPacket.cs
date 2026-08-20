using Il2CppSLZ.Data;
using UnityEngine;

using NEP.MonoDirector.Archetypes;

namespace NEP.MonoDirector.Events;

public sealed class HandGrabPacket : EventPacket
{
    public HandGrabPacket(List<AudioClip> clips, Vector3 position, float volume, float pitch, float spatial)
    {
        m_clips = clips;
        m_position = position;
        m_volume = volume;
        m_pitch = pitch;
        m_spatial = spatial;
    }

    private List<AudioClip> m_clips;
    private Vector3 m_position;
    private float m_volume;
    private float m_pitch;
    private float m_spatial;
    
    public override void Execute()
    {
        BoneLib.Audio.PlayAtPoint(
            m_clips.ToArray(),
            m_position,
            BoneLib.Audio.Footsteps,
            m_volume,
            m_pitch,
            m_spatial);
    }

    public override byte[] Serialize()
    {
        throw new NotImplementedException();
    }

    public override void Deserialize(Stream stream)
    {
        throw new NotImplementedException();
    }
}