using System.Collections.Generic;

namespace NEP.MonoDirector.Data
{
    public struct FrameGroup
    {
        public void SetFrames(ObjectFrame[] transformFrames, float frameTime)
        {
            this.transformFrames = new ObjectFrame[transformFrames.Length];
            this.frameTime = frameTime;

            for(int i = 0; i < this.transformFrames.Length; i++)
            {
                this.transformFrames[i] = transformFrames[i];
                transformFrames[i].SetDelta(frameTime);
            }
        }

        public ObjectFrame[] transformFrames;
        public float frameTime;
    }
}
