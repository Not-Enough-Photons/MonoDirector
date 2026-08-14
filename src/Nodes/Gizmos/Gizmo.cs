using Il2CppSLZ.Marrow;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;

namespace NEP.MonoDirector.Nodes.Gizmos;

[RegisterTypeInIl2Cpp]
public abstract class Gizmo(IntPtr ptr) : MonoBehaviour(ptr)
{
    public float Value => m_value;
    
    protected float m_value;
    protected TextMeshPro m_text;

    private Grip m_grip;
    
    private Action<Hand> m_OnHandAttached;
    private Action<Hand> m_OnHandDetached;
    private Action<Hand> m_OnHandUpdate;
    private Action m_OnAButtonPressed;
    private Action m_OnBButtonPressed;
    
    private void Awake()
    {
        m_OnHandAttached   = OnHandAttached;
        m_OnHandDetached   = OnHandDetached;
        m_OnHandUpdate     = OnHandUpdate;
        m_OnAButtonPressed = OnAButtonDown;
        m_OnBButtonPressed = OnBButtonDown;
    }

    private void OnEnable()
    {
        m_grip.attachedHandDelegate += m_OnHandAttached;
        m_grip.detachedHandDelegate += m_OnHandDetached;
        m_grip.attachedUpdateDelegate += m_OnHandUpdate;
    }

    private void OnDisable()
    {
        m_grip.attachedHandDelegate -= m_OnHandAttached;
        m_grip.detachedHandDelegate -= m_OnHandDetached;
        m_grip.attachedUpdateDelegate -= m_OnHandUpdate;
    }

    protected virtual void Start() { }
    protected virtual void OnHandAttached(Hand hand) { }
    protected virtual void OnHandDetached(Hand hand) { }

    protected virtual void OnHandUpdate(Hand hand)
    {
        if (hand.GetIndexButtonDown())
            OnPrimaryButtonDown();
        
        if (hand.Controller.GetSecondaryInteractionButtonDown())
            OnSecondaryButtonDown();
        
        if (hand.Controller.GetAButtonDown())
            OnAButtonDown();
        
        if (hand.Controller.GetBButtonDown())
            OnBButtonDown();
    }

    protected virtual void OnPrimaryButtonDown() { }
    protected virtual void OnSecondaryButtonDown() { }
    protected virtual void OnAButtonDown() { }
    protected virtual void OnBButtonDown() { }
}