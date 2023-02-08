using System;

namespace NEP.MonoDirector.Data
{
    public struct CompressedQuaternion
    {
        public CompressedQuaternion(short x, short y, short z, short w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public CompressedQuaternion(float x, float y, float z, float w)
        {
            this.x = Convert.ToInt16(x);
            this.y = Convert.ToInt16(y);
            this.z = Convert.ToInt16(z);
            this.w = Convert.ToInt16(w);
        }

        public short x;
        public short y;
        public short z;
        public short w;
    }
}
