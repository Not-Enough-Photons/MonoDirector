using UnityEngine;
using Il2CppSLZ.Marrow.Interaction;

using NEP.MonoDirector.Data;
using NEP.MonoDirector.Extensions;
using NEP.MonoDirector.Keyframes;

namespace NEP.MonoDirector.Archetypes;

// NOTE FOR THE FUTURE ABOUT OWNERSHIP:
// Ownership of props is very tricky to implement properly.
// Here are my ideas/criteria for ownership.
// ---------------------------------
// 1. Ungrabbed props are owned by the recording actor by default
// 2. If Actor B grabs it before Actor A, Actor B owns it (and vice versa)
// 3. If at least one recording actor hand is on the prop, they own it
// 4. If multiple actors are grabbing the same prop, whoever grabbed the prop first will still own it
// 5. If nobody is holding/grabbing the prop, nobody owns it
// 6. Ownership cannot be taken away from vehicles; it belongs to whomever propified it first
// 7. Unowned props are physical until grabbed, where they become kinematic
// 8. Ownership transfer of props must override their object frames
// ---------------------------------
// 
// OWNERSHIP THOUGHTS:
// - Mediation of ownership could be done through a class that listens for grabs on props
// - Actors could be refactored to have "actor hands" that also listen for grabs and can help fire events
// - Make this more efficient and not use as much code
// - Maybe not record actions for owning/disowning props

public sealed class Prop : Archetype
{
    public Prop(MarrowEntity entity)
    {
        m_entity = entity;
        Initialize();
    }

    public MarrowEntity Entity => m_entity;
    
    private MarrowEntity m_entity;
	
    public override void Initialize()
    {
        for (int i = 0; i < m_entity.Bodies.Count; i++)
        {
            MarrowBody body = m_entity.Bodies[i];
            AddPositionTrack(body.name);
            AddRotationTrack(body.name);
        }
    }

    public override void Destroy()
    {
        m_entity.Unfreeze();
        m_entity = null;
    }
	
    public override void SceneBegin()
    {
        m_entity.Freeze();
        
        for (int i = 0; i < m_entity.Bodies.Count; i++)
        {
            MarrowBody body = m_entity.Bodies[i];
            body.transform.position = m_positionTracks[i].FirstFrame.Value;
            body.transform.rotation = m_rotationTracks[i].FirstFrame.Value;
        }
    }
	
    public override void SceneEnd()
    {
        m_entity.Unfreeze();
        
        for (int i = 0; i < m_entity.Bodies.Count; i++)
        {
            MarrowBody body = m_entity.Bodies[i];
            body.transform.position = m_positionTracks[i].LastFrame.Value;
            body.transform.rotation = m_rotationTracks[i].LastFrame.Value;
        }
    }
	
    public override void Act(float time)
    {
        for (int i = 0; i < m_entity.Bodies.Count; i++)
        {
            MarrowBody body = m_entity.Bodies[i];

            KeyframeTrack<Vector3> positionTrack = m_positionTracks[i];
            KeyframeTrack<Quaternion> rotationTrack = m_rotationTracks[i];
            
            body.transform.position = Interpolator.EvaluatePosition(time, ref positionTrack);
            body.transform.rotation = Interpolator.EvaluateRotation(time, ref rotationTrack);
        }
    }
	
    public override void Capture(float time)
    {
        for (int i = 0; i < m_entity.Bodies.Count; i++)
        {
            MarrowBody body = m_entity.Bodies[i];
            m_positionTracks[i].Add(time, body.transform.position);
            m_rotationTracks[i].Add(time, body.transform.rotation);
        }

       //ProcessQueuedEvents(time);
    }
}