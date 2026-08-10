using NEP.MonoDirector.Data;
using NEP.MonoDirector.Keyframes;
using NEP.MonoDirector.Patches;

using UnityEngine;
using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Archetypes;

public class Actor : Archetype
{
    public Actor()
    {
        m_avatar = null;
    }

    public Actor(MarrowAvatar avatar)
    {
        SetAvatar(avatar);
        Initialize();
    }

    public MarrowAvatar Avatar => m_avatar;

    private readonly List<int> HeadBones =
    [
        (int)HumanBodyBones.Head,
        (int)HumanBodyBones.Jaw,
        (int)HumanBodyBones.LeftEye,
        (int)HumanBodyBones.RightEye
    ];
    
    private MarrowAvatar m_avatar;
    private List<Transform> m_bones;
    
    public sealed override void Initialize()
    {
        for (int i = 0; i < m_bones.Count; i++)
        {
            Transform bone = m_bones[i];

            if (!bone)
            {
                AddPositionTrack("Null");
                AddRotationTrack("Null");
            }
            else
            {
                AddPositionTrack(bone.name);
                AddRotationTrack(bone.name);
            }
        }
    }

    public override void Destroy()
    {
        GameObject.Destroy(m_avatar.gameObject);
        m_bones.Clear();
        m_positionTracks.Clear();
        m_rotationTracks.Clear();
    }

    public override void SceneBegin()
    {
        for (int i = 0; i < m_bones.Count; i++)
        {
            Transform bone = m_bones[i];

            if (!bone)
                continue;
            
            bone.transform.position = m_positionTracks[i].FirstFrame.Value;
            bone.transform.rotation = m_rotationTracks[i].FirstFrame.Value;
        }
    }

    public override void SceneEnd()
    {
        for (int i = 0; i < m_bones.Count; i++)
        {
            Transform bone = m_bones[i];

            if (!bone)
                continue;
            
            bone.transform.position = m_positionTracks[i].LastFrame.Value;
            bone.transform.rotation = m_rotationTracks[i].LastFrame.Value;
        }
    }

    public override void Act(float time)
    {
        for (int i = 0; i < m_bones.Count; i++)
        {
            Transform bone = m_bones[i];
            
            if (!bone)
                continue;
            
            KeyframeTrack<Vector3> positionTrack = m_positionTracks[i];
            KeyframeTrack<Quaternion> rotationTrack = m_rotationTracks[i];
            
            bone.transform.position = Interpolator.EvaluatePosition(time, ref positionTrack);
            bone.transform.rotation = Interpolator.EvaluateRotation(time, ref rotationTrack);
        }
    }

    public override void Capture(float time)
    {
        for (int i = 0; i < m_bones.Count; i++)
        {
            Transform bone = m_bones[i];
            
            if (!bone)
                continue;

            if ((HumanBodyBones)i == HumanBodyBones.Head
                && (HumanBodyBones)i == HumanBodyBones.Jaw
                || (HumanBodyBones)i == HumanBodyBones.LeftEye
                || (HumanBodyBones)i == HumanBodyBones.RightEye)
            {
                m_positionTracks[i].Add(time, bone.transform.position + PlayerAvatarArtPatches.UpdateAvatarHead.CalculatedHeadOffset);
                m_rotationTracks[i].Add(time, bone.transform.rotation);
                continue;
            }
            
            m_positionTracks[i].Add(time, bone.transform.position);
            m_rotationTracks[i].Add(time, bone.transform.rotation);
        }
    }

    public void SetAvatar(MarrowAvatar avatar)
    {
        m_avatar = avatar;
        SetBones(avatar);
    }
    
    private void SetBones(MarrowAvatar avatar)
    {
        if (!avatar)
            return;

        m_bones = new List<Transform>();

        for (HumanBodyBones index = HumanBodyBones.Hips; index != HumanBodyBones.LastBone; index++)
        {
            Transform bone = avatar.animator.GetBoneTransform(index);
            m_bones.Add(bone);
        }
    }
}