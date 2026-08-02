using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.UI;

using UnityEngine;
using UnityEngine.Playables;

namespace NEP.MonoDirector.Archetypes.Proxy;

[MelonLoader.RegisterTypeInIl2Cpp]
public class ActorProxy(IntPtr ptr) : MonoBehaviour(ptr)
{
    // For a traditional rig, this should be all the "head" bones
    public readonly List<int> HeadBones =
    [
        (int)HumanBodyBones.Head,
        (int)HumanBodyBones.Jaw,
        (int)HumanBodyBones.LeftEye,
        (int)HumanBodyBones.RightEye
    ];
    
    public Actor Actor { get => m_actor; }
    public BoxCollider Collider { get => m_triggerHull; }
    public bool Selected { get => m_selected; }
    
    private Actor m_actor;
    private BoxCollider m_triggerHull;
    private ActorFrame m_frame;
    private Marker m_marker;
    private bool m_selected;

    private Animator m_animator;
    private List<Transform> m_bones;

    private void BuildHull()
    {
        GameObject triggerHullObject = new GameObject("Actor Trigger Hull");
        m_triggerHull = triggerHullObject.AddComponent<BoxCollider>();
        m_triggerHull.isTrigger = true;
        m_triggerHull.size = new Vector3(0.5f, 1f, 0.5f);
        triggerHullObject.transform.SetParent(transform);
        triggerHullObject.transform.localPosition = Vector3.up;
        triggerHullObject.transform.localRotation = Quaternion.identity;
        triggerHullObject.SetActive(false);
    }

    public void PoseAtStart() => Pose(m_actor.StartFrame);

    public void PoseAtEnd() => Pose(m_actor.EndFrame);
    
    public void Pose(FrameGroup frame)
    {
        for (int i = 0; i < 55; i++)
        {
            var bone = m_bones[i];

            if (!bone)
                continue;
            
            bone.position = frame.TransformFrames[i].position;
            bone.rotation = frame.TransformFrames[i].rotation;
        }
    }

    public void Pose(FrameGroup previous, FrameGroup next)
    {
        float gap = next.FrameTime - previous.FrameTime;
        float head = Playback.PlaybackTime - previous.FrameTime;

        float delta = head / gap;

        ObjectFrame[] previousTransformFrames = previous.TransformFrames;
        ObjectFrame[] nextTransformFrames = next.TransformFrames;

        for (int i = 0; i < 55; i++)
        {
            if (i == (int)HumanBodyBones.Jaw)
                continue;

            if (previousTransformFrames == null)
                continue;

            Vector3 previousPosition = previousTransformFrames[i].position;
            Vector3 nextPosition = nextTransformFrames[i].position;

            Quaternion previousRotation = previousTransformFrames[i].rotation;
            Quaternion nextRotation = nextTransformFrames[i].rotation;

            var bone = m_bones[i];

            if (!bone)
                continue; 
            
            bone.position = Vector3.Lerp(previousPosition, nextPosition, delta);
            bone.rotation = Quaternion.Slerp(previousRotation, nextRotation, delta);
        }

        // m_microphone?.Playback();
        // m_microphone?.UpdateJaw();
    }

    public FrameGroup Record()
    {
        FrameGroup group = new FrameGroup();
        List<ObjectFrame> frames = new List<ObjectFrame>();
        
        for (int i = 0; i < 55; i++)
        {
            Transform bone = m_bones[i];

            if (!bone)
            {
                ObjectFrame nullFrame = new ObjectFrame();
                frames.Add(nullFrame);
                continue;
            }
            
            Vector3 position = bone.position;
            Quaternion rotation = bone.rotation;
            
            ObjectFrame frame = new ObjectFrame(position, rotation);
            
            frames.Add(frame);
        }

        group.SetFrames(frames.ToArray(), Recorder.RecordingTime);
        
        // Undo the head offset... afterward because no branching :P
        // This undoes it for every bone we count under it too!
        foreach (int headBone in HeadBones)
            group.TransformFrames[headBone].position += Patches.PlayerAvatarArtPatches.UpdateAvatarHead.calculatedHeadOffset;
        
        return group;
    }

    public void SetAnimator(Animator animator)
    {
        m_bones = new List<Transform>();
        m_animator = animator;
        int numBones = (int)HumanBodyBones.LastBone;

        for (int i = 0; i < numBones; i++)
        {
            Transform bone = animator.GetBoneTransform((HumanBodyBones)i);

            m_bones.Add(bone);
        }
    }
    
    public void SetActor(Actor actor)
    {
        BuildHull();
        
        m_actor = actor;
        
        m_triggerHull.transform.SetParent(m_actor.Avatar.animator.GetBoneTransform(HumanBodyBones.Hips));

        if (m_frame == null)
        {
            m_frame = ActorFrameManager.AddFrameToActor(this);
            MarkerManager.AddMarkerToActor(this);
            m_frame.SetSize(m_triggerHull.size);
            m_frame.Hide();
        }
        else
        {
            m_frame.SetSize(m_triggerHull.size);
            m_frame.Hide();
        }
    }

    public void SetMarker(Marker marker)
    {
        m_marker = marker;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnSelected()
    {
        m_triggerHull.gameObject.SetActive(true);
        Director.SelectActor(m_actor);
        m_frame.Show();
        m_selected = true;
    }

    public void OnDeselected()
    {
        m_triggerHull.gameObject.SetActive(false);
        Director.DeselectActor(m_actor);
        m_frame.Hide();
        m_selected = false;
    }

    public void OnHidden() => m_marker.OnActorUpdated(m_actor);
}
