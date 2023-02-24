using System.Collections.Generic;

namespace NEP.MonoDirector.Data
{
    public struct FrameGroup
    {
        public FrameGroup(List<ObjectFrame> frames)
        {
            this.transformFrames = frames;
        }

        public void SetFrames(List<ObjectFrame> transformFrames)
        {
            this.transformFrames = transformFrames;
        }

        public List<ObjectFrame> transformFrames;
    }
}
