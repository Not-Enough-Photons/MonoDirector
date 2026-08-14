using Il2CppSLZ.Marrow;
using MelonLoader;

namespace NEP.MonoDirector.Nodes.Gizmos;

[RegisterTypeInIl2Cpp]
public sealed class TetherGizmo(IntPtr ptr) : Gizmo(ptr)
{
    protected override void OnHandAttached(Hand hand)
    {
        throw new NotImplementedException();
    }

    protected override void OnHandDetached(Hand hand)
    {
        throw new NotImplementedException();
    }
}