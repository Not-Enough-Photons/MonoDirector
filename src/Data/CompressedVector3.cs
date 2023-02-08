using System;

namespace NEP.MonoDirector.Data
{
    public struct CompressedVector3
    {
        public CompressedVector3(short x, short y, short z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public CompressedVector3(float x, float y, float z)
        {
            this.x = Convert.ToInt16(x);
            this.y = Convert.ToInt16(y);
            this.z = Convert.ToInt16(z);
        }

        public short x;
        public short y;
        public short z;
    }
}
