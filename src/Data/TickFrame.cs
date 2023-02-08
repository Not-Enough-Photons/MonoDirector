using System.Collections.Generic;

namespace NEP.MonoDirector.Data
{
    public struct TickFrame
    {
        public TickFrame(List<ObjectFrame> frames)
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
