using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.Data;

/// <summary>
/// A collection of frames, this is useful for actors who generate many frames at once
/// Instead of using each individual frames' timestamp we just use the group!
/// </summary>
public struct FrameGroup : IBinaryData
{
    public void SetFrames(ObjectFrame[] transformFrames, float frameTime)
    {
        this.TransformFrames = new ObjectFrame[transformFrames.Length];
        this.FrameTime = frameTime;

        for (int i = 0; i < this.TransformFrames.Length; i++)
        {
            this.TransformFrames[i] = transformFrames[i];
            transformFrames[i].SetDelta(frameTime);
        }
    }
    
    public ObjectFrame[] TransformFrames;
    public float FrameTime;
    
    //
    // Enums
    //
    public enum VersionNumber : short
    {
        V1
    }
    
    //
    // IBinaryData
    //
    public byte[] ToBinary()
    {
        List<byte> bytes = new List<byte>();
        
        // V1 header data is structured as so...
        //
        // version : u16
        // frame_time : float
        // num_frames : i32
        //
        // Below is num_frames number of ObjectFrames
        bytes.AddRange(BitConverter.GetBytes((short)VersionNumber.V1));
        bytes.AddRange(BitConverter.GetBytes(FrameTime));
        bytes.AddRange(BitConverter.GetBytes(TransformFrames.Length));
        
        foreach (ObjectFrame frame in TransformFrames)
            bytes.AddRange(frame.ToBinary());

        return bytes.ToArray();
    }
    
    public void FromBinary(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        // Check the version number
        short version = reader.ReadInt16();

        if (version != (short)VersionNumber.V1)
            throw new Exception($"Unsupported version type! Value was {version}");
        
        // Deserialize
        if (version == (short)VersionNumber.V1)
        {
            float frameTime = reader.ReadSingle();
            int numFrames = reader.ReadInt32();

            FrameTime = frameTime;
            
#if DEBUG
            Logging.Msg($"[ACTOR]: GrFrameDT is {FrameTime}");
            Logging.Msg($"[ACTOR]: GrOfCount is {numFrames}");
#endif
            
            TransformFrames = new ObjectFrame[numFrames];

            for (int f = 0; f < numFrames; f++)
            {
                TransformFrames[f] = new ObjectFrame();
                TransformFrames[f].FromBinary(stream);
            }
        }
    }

    // FGDB - Frame Group Data Block
    public uint GetBinaryID() => 0x42444746;
}
