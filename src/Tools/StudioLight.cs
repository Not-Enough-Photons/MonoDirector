using SLZ.Bonelab;
using SLZ.Interaction;
using System;
using UnityEngine;

namespace NEP.MonoDirector.Tools
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class StudioLight : MonoBehaviour
    {
        public StudioLight(System.IntPtr ptr) : base(ptr) { }

        private handLightControl handLightcontrol
        {
            get
            {
                if(_handLightControl == null)
                {
                    _handLightControl = GetComponent<handLightControl>();
                }

                return _handLightControl;
            }
        }

        private handLightControl _handLightControl;

        private Light light => this.handLightcontrol.handLight;
        private Color[] colors => this.handLightcontrol.color;
        private float[] intensities => this.handLightcontrol.intensity;
        private Grip grip => this.handLightcontrol.grip;

        private int colorIndex;
        private int intensityIndex;

        private void Awake()
        {
            grip.attachedUpdateDelegate += new Action<Hand>((hand) => HandAttachedUpdate(hand));
        }

        private void HandAttachedUpdate(Hand hand)
        {
            if (hand._indexButtonDown)
            {
                light.color = colors[colorIndex];

                if (colorIndex + 1 >= colors.Length)
                {
                    colorIndex = 0;
                }
                else
                {
                    colorIndex++;
                }
            }

            if (hand.Controller.GetMenuTap())
            {
                light.intensity = intensities[intensityIndex];

                if (intensityIndex + 1 >= colors.Length)
                {
                    intensityIndex = 0;
                }
                else
                {
                    intensityIndex++;
                }
            }
        }
    }
}
