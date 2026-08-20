using System.Runtime.InteropServices;
using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Serialization;

namespace NEP.MonoDirector.Events;

public sealed class GunSlidePacket : EventPacket
{
    public override byte ID => (byte)EventType.GUN_SLIDE;
    
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