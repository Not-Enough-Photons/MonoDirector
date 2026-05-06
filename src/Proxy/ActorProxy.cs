using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.UI;

using UnityEngine;

namespace NEP.MonoDirector.Proxy;

[MelonLoader.RegisterTypeInIl2Cpp]
public class ActorProxy(IntPtr ptr) : TrackableProxy(ptr)
{
    public Actor Actor { get => m_actor; }
    public BoxCollider Collider { get => m_triggerHull; }
    public bool Selected { get => m_selected; }
    
    private Actor m_actor;
    private BoxCollider m_triggerHull;
    private ActorFrame m_frame;
    private Marker m_marker;
    private bool m_selected;

    private void Awake()
    {
        BuildHull();
    }

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

    public void SetActor(Actor actor)
    {
        m_actor = actor;
        
        m_triggerHull.transform.SetParent(m_actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.Hips));

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

    public void OnSelected()
    {
        FeedbackSFX.LinkAudio();
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
