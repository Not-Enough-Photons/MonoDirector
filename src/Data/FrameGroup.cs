using System.Collections.Generic;

namespace NEP.MonoDirector.Data
{
    public class FrameGroup
    {
        public FrameGroup(ObjectFrame[] frames)
        {
            this.transformFrames = frames;
        }

        public FrameGroup(List<ObjectFrame> frames)
        {
            this.transformFrames = frames.ToArray();
        }

        public void SetFrames(List<ObjectFrame> transformFrames)
        {
            this.transformFrames = transformFrames.ToArray();
        }

        public ObjectFrame[] transformFrames;

        public FrameGroup previous;
        public FrameGroup next;
    }
}
