using NEP.MonoDirector.Serialization;

namespace NEP.MonoDirector.Events;

public sealed class MuzzleFlashPacket : EventPacket, ISerialized
{
    public override byte ID => (byte)EventType.MUZZLE_FLASH;
    
    public byte[] Serialize()
    {
        throw new NotImplementedException();
    }

    public void Deserialize(Stream stream)
    {
        throw new NotImplementedException();
    }
    
    public override void Execute()
    {
        throw new NotImplementedException();
    }
}