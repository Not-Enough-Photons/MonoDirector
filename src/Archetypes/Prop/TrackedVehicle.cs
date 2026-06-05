using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Archetypes;

public class TrackedVehicle : Prop
{
    public Atv Vehicle { get => m_vehicle; }

    protected Atv m_vehicle;

    public void SetVehicle(Atv vehicle)
    {
        this.m_vehicle = vehicle;
    }

    public void RemoveVehicle()
    {
        m_vehicle = null;

        //SetPhysicsActive(true);
    }

    public override void OnSceneBegin()
    {
        if (m_frames == null)
            return;

        if (m_frames.Count == 0)
            return;

        //SetPhysicsActive(false);
    }
}
