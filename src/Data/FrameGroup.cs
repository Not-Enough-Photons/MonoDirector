using System.Collections.Generic;

namespace NEP.MonoDirector.Data
{
    public struct FrameGroup
    {
        public FrameGroup(List<ObjectFrame> frames)
        {
            this.transformFrames = frames;
            frameTime = 0f;
        }

        public void SetFrames(List<ObjectFrame> transformFrames, float frameTime)
        {
            this.frameTime = frameTime;
            this.transformFrames = transformFrames;

            for(int i = 0; i < transformFrames.Count; i++)
            {
                transformFrames[i].SetDelta(frameTime);
            }
        }

        public List<ObjectFrame> transformFrames;
        public float frameTime;
    }
}
