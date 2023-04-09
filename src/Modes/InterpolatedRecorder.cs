using NEP.MonoDirector.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NEP.MonoDirector.Core
{
    public class InterpolatedRecorder
    {
        private float currentFrameDelta;
        
        public void Record()
        {
            currentFrameDelta = Time.deltaTime;
        }
    }
}
