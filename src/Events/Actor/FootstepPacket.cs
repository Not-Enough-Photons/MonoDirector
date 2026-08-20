using Il2CppSLZ.Data;
using Il2CppSLZ.Marrow.Audio;
using NEP.MonoDirector.Archetypes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NEP.MonoDirector.Events;

public sealed class FootstepPacket : EventPacket
{
    public FootstepPacket(Actor actor, Vector3 position, float volume, float pitch, float spatial, bool jogging)
    {
        m_actor = actor;
        m_position = position;
        m_volume = volume;
        m_pitch = pitch;
        m_spatial = spatial;
        m_isJogging = jogging;
    }

    private Actor m_actor;
    private Vector3 m_position;
    private float m_volume;
    private float m_pitch;
    private float m_spatial;
    private bool m_isJogging;
    
    public override void Execute()
    {
        var avatar = m_actor.Avatar;
        AudioVarianceData footsteps = m_isJogging ? avatar.footstepsJog : avatar.footstepsWalk;
        var clips = footsteps.audioClips.ToArray();
        
        BoneLib.Audio.PlayAtPoint(clips, m_position, BoneLib.Audio.Footsteps, m_volume, m_pitch, m_spatial);
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