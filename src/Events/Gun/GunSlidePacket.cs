using System.Runtime.InteropServices;
using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Serialization;

namespace NEP.MonoDirector.Events;

[StructLayout(LayoutKind.Sequential)]
public sealed class GunSlidePacket : EventPacket, ISerialized
{
    public override byte ID => (byte)EventType.GUN_SLIDE;
    
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