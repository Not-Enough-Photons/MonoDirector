using NEP.MonoDirector.Serialization;

namespace NEP.MonoDirector.Events;

public sealed class MuzzleFlashPacket : EventPacket
{
    public override byte ID => (byte)EventType.GUN_MUZZLE_FLASH;
    
    public override void Execute()
    {
        throw new NotImplementedException();
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