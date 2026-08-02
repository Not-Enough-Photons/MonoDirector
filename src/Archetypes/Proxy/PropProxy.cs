using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;

using MelonLoader;
using UnityEngine;

using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.Archetypes.Proxy;

[RegisterTypeInIl2Cpp]
public class PropProxy(IntPtr ptr) : ArchetypeProxy(ptr)
{
    public Prop Prop => m_prop;
    public MarrowEntity Entity => m_entity;
    
    private Prop m_prop;

    private MarrowEntity m_entity;
    private List<InteractableHost> m_interactableHosts;

    private Action<InteractableHost, Hand> m_onHandAttached;
    private Action<InteractableHost, Hand> m_onHandDetached;

    private void Awake()
    {
        m_onHandAttached = OnHandAttached;
        m_onHandDetached = OnHandDetached;
        
        m_interactableHosts = new List<InteractableHost>();
        
        if (!TryGetComponent(out MarrowEntity entity))
            throw new NullReferenceException("Failed to find MarrowEntity on a PropProxy!");

        m_entity = entity;
        
        var behaviours = m_entity._behaviours;
        
        foreach (var behaviour in behaviours)
        {
            if (!behaviour)
                continue;
            
            InteractableHost interactableHost = behaviour.GetComponent<InteractableHost>();
            
            if (!interactableHost)
                continue;
            
            m_interactableHosts.Add(interactableHost);
        }
        
        foreach (var host in m_interactableHosts)
        {
            host.onHandAttachedDelegate += m_onHandAttached;
            host.onHandDetachedDelegate += m_onHandDetached;
        }
    }

    private void OnDestroy()
    {
        foreach (var host in m_interactableHosts)
        {
            host.onHandAttachedDelegate -= m_onHandAttached;
            host.onHandDetachedDelegate -= m_onHandDetached;
        }
    }

    public void Show() => gameObject.SetActive(true);

    public void Hide() => gameObject.SetActive(false);

    public void SetProp(Prop prop)
    {
        m_prop = prop;
    }
    
    public void SetPhysicsActive(bool enable)
    {
        foreach (var body in m_entity.Bodies)
        {
            if (body.TryGetRigidbody(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = !enable;
            }
        }
    }

    public void PoseAtStart() => Pose(Prop.StartFrame);

    public void PoseAtEnd() => Pose(Prop.EndFrame);

    public void Pose(FrameGroup frame)
    {
        for (int i = 0; i < m_entity.Bodies.Count; i++)
        {
            var body = m_entity.Bodies[i];

            if (!body)
                continue;

            body.transform.position = frame.TransformFrames[i].position;
            body.transform.rotation = frame.TransformFrames[i].rotation;
        }
    }
    
    public void Pose(FrameGroup previous, FrameGroup next)
    {
        float gap = next.FrameTime - previous.FrameTime;
        float head = Playback.PlaybackTime - previous.FrameTime;

        float delta = head / gap;

        ObjectFrame[] previousTransformFrames = previous.TransformFrames;
        ObjectFrame[] nextTransformFrames = next.TransformFrames;

        for (int i = 0; i < m_entity.Bodies.Count; i++)
        {
            if (previousTransformFrames == null)
                continue;

            Vector3 previousPosition = previousTransformFrames[i].position;
            Vector3 nextPosition = nextTransformFrames[i].position;

            Quaternion previousRotation = previousTransformFrames[i].rotation;
            Quaternion nextRotation = nextTransformFrames[i].rotation;

            MarrowBody body = m_entity.Bodies[i];

            body.transform.position = Vector3.Lerp(previousPosition, nextPosition, delta);
            body.transform.rotation = Quaternion.Slerp(previousRotation, nextRotation, delta);

#if ENABLE_OWNERSHIP
            if (m_isOwned)
            {
                body._rigidbody.isKinematic = true;
                body.transform.position = Vector3.Lerp(previousPosition, nextPosition, delta);
                body.transform.rotation = Quaternion.Slerp(previousRotation, nextRotation, delta);
            }
            else
            {
                if (body._rigidbody.isKinematic)
                {
                    body._rigidbody.isKinematic = false;
                    body.AddForce(nextTransformFrames[i].rigidbodyVelocity, ForceMode.VelocityChange);
                    body.AddTorque(nextTransformFrames[i].rigidbodyAngularVelocity, ForceMode.VelocityChange);
                }
            }
#endif
        }
    }

    public FrameGroup Record()
    {
        FrameGroup group = new FrameGroup();
        List<ObjectFrame> objectFrames = new List<ObjectFrame>();

        foreach (var body in m_entity.Bodies)
        {
            ObjectFrame objectFrame = new ObjectFrame()
            {
                transform = body.transform,
                position = body.transform.position,
                rotation = body.transform.rotation,
                scale = body.transform.localScale,
#if ENABLE_OWNERSHIP
                rigidbodyVelocity = body._rigidbody.velocity,
                rigidbodyAngularVelocity = body._rigidbody.angularVelocity,
#endif
                frameTime = Recorder.RecordingTime
            }; 
            
            objectFrames.Add(objectFrame);
        }

        group.SetFrames(objectFrames.ToArray(), Recorder.RecordingTime);
        return group;
    }

    private void OnHandAttached(InteractableHost host, Hand hand)
    {
        
    }
    
    private void OnHandDetached(InteractableHost host, Hand hand)
    {
        
    }
}