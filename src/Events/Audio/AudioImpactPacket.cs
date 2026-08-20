using UnityEngine;

namespace NEP.MonoDirector.Events.Audio;

public sealed class AudioImpactPacket : EventPacket
{
    public AudioImpactPacket(List<AudioClip> clips, Vector3 position, float volume, float pitch, float spatial)
    {
        m_clips = clips;
        m_position = position;
        m_volume = volume;
        m_pitch = pitch;
        m_spatial = spatial;
    }

    public override byte ID => (byte)EventType.IMPACT_SFX;

    private List<AudioClip> m_clips;
    private Vector3 m_position;
    private float m_volume;
    private float m_pitch;
    private float m_spatial;
    
    public override void Execute()
    {
        BoneLib.Audio.PlayAtPoint(m_clips.ToArray(), m_position, BoneLib.Audio.Impact, m_volume, m_pitch, m_spatial);
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