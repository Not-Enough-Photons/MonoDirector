using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Splines;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SplinePlacer : MonoBehaviour
    {
        public SplinePlacer(System.IntPtr ptr) : base(ptr) { }

        private SplineContainer splineContainer;
        private SplineAnimate splineAnimator;

        private Transform lookAtTarget;

        private void Start()
        {
            splineContainer = CreateSplineContainer();
            InitializeSplineAnimator(splineAnimator);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                CreatePoint(transform.position, transform.rotation);
            }

            if(lookAtTarget != null)
            {
                transform.LookAt(lookAtTarget);
            }
        }

        private void CreatePoint(Vector3 position, Quaternion rotation)
        {
            if(splineContainer.Spline == null)
            {
                splineContainer.Spline = new Spline();
            }

            splineContainer.Spline.m_EditModeType = SplineType.CatmullRom;

            BezierKnot knot = new BezierKnot();

            knot.Position = position;
            knot.Rotation = rotation;

            splineContainer.Spline.Add(knot);
            print("Spline knot added!");
        }

        private SplineContainer CreateSplineContainer()
        {
            if(splineContainer == null)
            {
                GameObject splineContainerObject = new GameObject("Spline Container");
                splineContainer = splineContainerObject.AddComponent<SplineContainer>();
            }

            return splineContainer;
        }

        private void InitializeSplineAnimator(SplineAnimate animator)
        {
            if(animator != null)
            {
                return;
            }

            animator = gameObject.AddComponent<SplineAnimate>();

            animator.alignmentMode = SplineAnimate.AlignmentMode.None;
            animator.method = SplineAnimate.Method.Speed;
            animator.easingMode = SplineAnimate.EasingMode.EaseOut;
            animator.objectForwardAxis = SplineComponent.AlignAxis.XAxis;
            animator.splineContainer = splineContainer;

            splineAnimator = animator;
        }
    }
}