using Il2CppSLZ.Marrow;
using MelonLoader;
using UnityEngine;

namespace NEP.MonoDirector.Nodes;

[RegisterTypeInIl2Cpp]
public abstract class Node(IntPtr ptr) : MonoBehaviour(ptr)
{
    protected Rigidbody m_rigidbody;
    protected Grip m_grip;
    protected GameObject m_frame;

    private Action<Hand> m_onHandAttached;
    private Action<Hand> m_onHandDetached;

    protected virtual void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_frame = transform.Find("Frame").gameObject;
        m_grip = transform.Find("Grip").GetComponent<Grip>();

        m_onHandAttached = OnHandAttached;
        m_onHandDetached = OnHandDetached;
    }

    protected virtual void OnEnable()
    {
        m_grip.attachedHandDelegate += m_onHandAttached;
        m_grip.detachedHandDelegate += m_onHandDetached;
    }

    protected virtual void OnDisable()
    {
        m_grip.attachedHandDelegate -= m_onHandAttached;
        m_grip.detachedHandDelegate -= m_onHandDetached;
    }

    protected virtual void OnHandAttached(Hand hand)
    {
        m_rigidbody.isKinematic = false;
    }

    protected virtual void OnHandDetached(Hand hand)
    {
        if (GetAttachedHands() > 1)
            return;

        m_rigidbody.isKinematic = true;
    }

    protected int GetAttachedHands()
    {
        return m_grip.attachedHands.Count;
    }
}