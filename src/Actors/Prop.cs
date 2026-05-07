using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

using UnityEngine;

using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;
using NEP.MonoDirector.State;

namespace NEP.MonoDirector.Actors;

[MelonLoader.RegisterTypeInIl2Cpp]
public class Prop(IntPtr ptr) : MonoBehaviour(ptr)
{
    public Trackable Actor => m_actor;
    public IReadOnlyList<FrameGroup> PropFrames => m_propFrames.AsReadOnly();
    public MarrowEntity Entity => m_entity;

    public bool IsRecording => m_isRecording;

    public static readonly Il2CppSystem.Type[] TypeWhitelist =
    {
        Il2CppInterop.Runtime.Il2CppType.Of<Gun>(),
        Il2CppInterop.Runtime.Il2CppType.Of<Magazine>(),
        Il2CppInterop.Runtime.Il2CppType.Of<ObjectDestructible>(),
        Il2CppInterop.Runtime.Il2CppType.Of<Atv>()
    };

    protected Trackable m_actor;
    protected MarrowEntity m_entity;

    protected int m_stateTick;
    protected int m_recordedTicks;

    protected List<FrameGroup> m_propFrames;
    protected List<ObjectFrame> m_bodyFrames;
    protected List<ActionFrame> m_actionFrames;

    protected List<InteractableHost> m_interactableHosts;

    protected bool m_isRecording;

    private FrameGroup m_previousFrame;
    private FrameGroup m_nextFrame;

    private Action<InteractableHost, Hand> m_onHandAttached;
    private Action<InteractableHost, Hand> m_onHandDetached;

    private bool m_isOwned;

    protected virtual void Awake()
    {
        m_propFrames = new List<FrameGroup>();
        m_bodyFrames = new List<ObjectFrame>();
        m_actionFrames = new List<ActionFrame>();
        m_interactableHosts = new List<InteractableHost>();

        m_onHandAttached = OnHandAttached;
        m_onHandDetached = OnHandDetached;
    }

    private void OnDisable()
    {
        if (m_interactableHosts == null || m_interactableHosts.Count == 0)
            return;
        
        foreach (var host in m_interactableHosts)
        {
            host.onHandAttachedDelegate -= m_onHandAttached;
            host.onHandDetachedDelegate -= m_onHandDetached;
        }
    }

    public static bool IsActorProp(MarrowEntity entity)
    {
        if (!entity)
            return false;

        if (entity.gameObject.layer == LayerMask.NameToLayer("EnemyColliders"))
            return false;

        // Prop already exists
        if (entity.GetComponent<Prop>())
            return false;

        return true;
    }

    public static bool EligibleWithType<T>(MarrowEntity entity)
    {
        return entity.GetComponent<T>() != null;
    }

    public void DeleteAllFrames()
    {
        m_propFrames.Clear();
        m_bodyFrames.Clear();
        m_actionFrames.Clear();
    }

    public void SetEntity(MarrowEntity entity)
    {
        m_entity = entity;

        if (!m_entity)
            return;
        
        var behaviours = m_entity._behaviours;

        foreach (var behaviour in behaviours)
        {
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

    public void SetActor(Trackable actor)
    {
        this.m_actor = actor;
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

    public virtual void OnSceneBegin()
    {
        if(PropFrames == null)
        {
            return;
        }

        if(PropFrames.Count == 0)
        {
            return;
        }

        for (int i = 0; i < m_entity.Bodies.Count; i++)
        {
            var body = m_entity.Bodies[i];

            if (body == null)
            {
                continue;
            }

            body.transform.position = m_propFrames[0].TransformFrames[i].position;
            body.transform.rotation = m_propFrames[0].TransformFrames[i].rotation;
        }

        SetPhysicsActive(false);

        foreach (var actionFrame in m_actionFrames)
        {
            actionFrame.Reset();
        }
    }

    public virtual void Act()
    {
        m_previousFrame = new FrameGroup();
        m_nextFrame = new FrameGroup();

        for (int i = 0; i < m_propFrames.Count; i++)
        {
            var frame = m_propFrames[i];

            m_previousFrame = m_nextFrame;
            m_nextFrame = frame;

            if (frame.FrameTime > Playback.Instance.PlaybackTime)
            {
                break;
            }
        }

        float gap = m_nextFrame.FrameTime - m_previousFrame.FrameTime;
        float head = Playback.Instance.PlaybackTime - m_previousFrame.FrameTime;

        float delta = head / gap;

        ObjectFrame[] previousTransformFrames = m_previousFrame.TransformFrames;
        ObjectFrame[] nextTransformFrames = m_nextFrame.TransformFrames;

        for (int i = 0; i < m_entity.Bodies.Count; i++)
        {
            if (previousTransformFrames == null)
                continue;

            Vector3 previousPosition = previousTransformFrames[i].position;
            Vector3 nextPosition = nextTransformFrames[i].position;

            Quaternion previousRotation = previousTransformFrames[i].rotation;
            Quaternion nextRotation = nextTransformFrames[i].rotation;

            MarrowBody body = m_entity.Bodies[i];

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
        }

        foreach(var actionFrame in m_actionFrames)
        {
            if (Playback.Instance.PlaybackTime < actionFrame.timestamp)
            {
                continue;
            }
            else
            {
                actionFrame.Run();
            }
        }
    }

    public virtual void Record(int frame)
    {
        if (m_entity == null)
            return;

        m_isRecording = true;

        List<ObjectFrame> objectFrames = new List<ObjectFrame>();
        FrameGroup group = new FrameGroup();

        foreach (var body in m_entity.Bodies)
        {
            ObjectFrame objectFrame = new ObjectFrame()
            {
                transform = body.transform,
                position = body.transform.position,
                rotation = body.transform.rotation,
                scale = body.transform.localScale,
                rigidbodyVelocity = body._rigidbody.velocity,
                rigidbodyAngularVelocity = body._rigidbody.angularVelocity,
                frameTime = Recorder.Instance.RecordingTime
            };

            if (frame == 0)
            {
                objectFrames.Add(objectFrame);
                foreach (var host in m_interactableHosts)
                {
                    if (host.HandCount() != 0)
                    {
                        RecordAction(() => Own());
                        break;
                    }
                }
            }
            else
            {
                objectFrames.Add(objectFrame);
                m_recordedTicks++;
            }

            group.SetFrames(objectFrames.ToArray(), Recorder.Instance.RecordingTime);
        }

        m_propFrames.Add(group);
    }

    public virtual void RecordAction(Action action)
    {
        RecordAction(action, Recorder.Instance.RecordingTime);
    }

    public virtual void RecordAction(Action action, float time = 0f)
    {
        if (Director.PlayState == State.PlayState.Recording)
        {
            if (!Caster.RecordProps.Contains(this))
            {
                return;
            }

            m_actionFrames.Add(new ActionFrame(action, time));
        }
    }

    public void ResetTicks()
    {
        m_stateTick = 0;
    }

    private void OnHandAttached(InteractableHost host, Hand hand)
    {
        if (Director.PlayState != PlayState.Recording)
            return;
        
        RecordAction(() => Own());
        
        Logging.Msg("OnHandAttached");
    }

    private void OnHandDetached(InteractableHost host, Hand hand)
    {
        if (Director.PlayState != PlayState.Recording)
            return;

        RecordAction(() => Disown());
        
        Logging.Msg("OnHandDetached");
    }

    // NOTE FOR THE FUTURE ABOUT OWNERSHIP:
    // Ownership of props is very tricky to implement properly.
    // Here is my ideas/criteria for ownership.
    // ---------------------------------
    // 1. Ungrabbed props are owned by the recording actor by default
    // 2. If Actor B grabs it before Actor A, Actor B owns it (and vice versa)
    // 3. If at least one recording actor hand is on the prop, they own it
    // 4. If multiple actors are grabbing the same prop, whoever grabbed the prop first will still own it
    // 5. If nobody is holding/grabbing the prop, nobody owns it
    // 6. Ownership cannot be taken away from vehicles; it belongs to whomever propified it first
    
    private void Own()
    {
        m_isOwned = true;
    }

    private void Disown()
    {
        m_isOwned = false;
    }
}
