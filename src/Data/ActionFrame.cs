using NEP.MonoDirector.Archetypes;

namespace NEP.MonoDirector.Data;

public class ActionFrame : IBinaryData
{
    public ActionFrame()
    {
        action = null;
        timestamp = 0f;
        type = 0;
    }
    
    public ActionFrame(byte type, Action action, float timestamp)
    {
        this.action = action;
        this.timestamp = timestamp;
        this.type = type;
    }

    public Action action;
    public float timestamp;
    public byte type;

    private bool runOnce = false;

    public void Reset()
    {
        runOnce = false;
    }

    public void Run()
    {
        if (runOnce == false)
        {
            action?.Invoke();
            runOnce = true;
        }
    }

    public byte[] ToBinary()
    {
        using MemoryStream stream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(stream);
        
        writer.Write(type);
        writer.Write(timestamp);

        return stream.ToArray();
    }

    public void FromBinary(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        type = reader.ReadByte();
        timestamp = reader.ReadSingle();
    }

    public uint GetBinaryID()
    {
        throw new NotImplementedException();
    }
}
