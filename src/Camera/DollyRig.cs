using System;
using NEP.MonoDirector.Core;
using UnityEngine;
using UnityEngine.Splines;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class DollyRig : FreeCameraRig
    {
        public DollyRig(IntPtr ptr) : base(ptr) { }

        private Spline spline;
        private SplineAnimate splineAnimator;

        private int currentFrame;
        private int recordedFrames;

        protected override void Awake()
        {
            splineAnimator = gameObject.AddComponent<SplineAnimate>();

            InitializeSpline();
        }

        protected override void Update()
        {
            if(Director.PlayState == State.PlayState.Playing)
            {
                splineAnimator.Play();
                return;
            }

            base.Update();

            if (Input.GetKeyDown(KeyCode.K))
            {
                AddCurve(transform.position, transform.rotation);
            }
        }

        public void AddCurve(Vector3 position, Quaternion rotation)
        {
            if(Director.PlayState != State.PlayState.Stopped)
            {
                return;
            }

            spline.Add(new BezierKnot(position, Vector3.one, Vector3.one, rotation));
            Main.Logger.Msg("Curve added!");
        }

        public void InitializeSpline()
        {
            if(spline == null)
            {
                spline = new Spline();
            }

            splineAnimator.splineContainer = new SplineContainer();
            splineAnimator.splineContainer.Spline = spline;
        }
    }
}
