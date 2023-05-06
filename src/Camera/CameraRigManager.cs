using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEP.MonoDirector.Cameras
{
    public class CameraRigManager
    {
        public CameraRigManager()
        {
            Start();
        }

        public static CameraRigManager instance { get; private set; }

        private void Start()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                instance = null;
            }
        }
    }
}
