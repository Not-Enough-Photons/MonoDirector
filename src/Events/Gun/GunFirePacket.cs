using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Events;

public sealed class GunFirePacket : EventPacket
{
    public GunFirePacket(Gun gun)
    {
        m_gun = gun;
    }

    public override byte ID => (byte)EventType.GUN_FIRE;

    private Gun m_gun;

    public override void Execute()
    {
        m_gun.OnFire();
    }

    public override byte[] Serialize()
    {
        throw new NotImplementedException();
    }

    public override void Deserialize(Stream stream)
    {
        throw new NotImplementedException();
    }
}
