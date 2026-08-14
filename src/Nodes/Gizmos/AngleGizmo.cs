using Il2CppSLZ.Marrow;
using MelonLoader;
using UnityEngine;

namespace NEP.MonoDirector.Nodes.Gizmos;

[RegisterTypeInIl2Cpp]
public sealed class AngleGizmo(IntPtr ptr) : Gizmo(ptr)
{
    protected override void OnHandAttached(Hand hand)
    {
        throw new NotImplementedException();
    }

    protected override void OnHandDetached(Hand hand)
    {
        throw new NotImplementedException();
    }
    
    protected override void OnHandUpdate(Hand hand)
    {
        m_value = Vector3.Angle(transform.localPosition, Vector3.forward);
        m_text.text = m_value.ToString("0.00") + " degrees";
    }
}