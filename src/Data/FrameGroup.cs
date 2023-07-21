using System;
using UnityEngine;

using System.Collections.Generic;
using System.IO;

namespace NEP.MonoDirector.Data
{
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
            // num_frames : i32
            // frame_time : float
            //
            // Below is num_frames number of ObjectFrames
            bytes.AddRange(BitConverter.GetBytes((short)VersionNumber.V1));
            bytes.AddRange(BitConverter.GetBytes(TransformFrames.Length));
            bytes.AddRange(BitConverter.GetBytes(FrameTime));
            
            foreach (ObjectFrame frame in TransformFrames)
                bytes.AddRange(frame.ToBinary());

            return bytes.ToArray();
        }
        
        public void FromBinary(Stream stream)
        {
            throw new System.NotImplementedException();
        }

        // FGDB - Frame Group Data Block
        public uint GetBinaryID() => 0x42444746;
    }
}
