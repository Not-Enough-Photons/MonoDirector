using SLZ.Interaction;
using UnityEngine;
using UnityEngine.Splines;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SplineNode : MonoBehaviour
    {
        public SplineNode(System.IntPtr ptr) : base(ptr) { }

        public static int nodeCounter { get; private set; }

        private SplineContainer splineContainer;
        private SplineAnimate splineAnimator;

        private Transform lookAtTarget;

        private SphereGrip nodeGrip;

        private int nodeIndex = 0;

        private void Start()
        {
            splineContainer = CreateSplineContainer();
            InitializeSplineAnimator(splineAnimator);

            nodeGrip = transform.Find("Collider").GetComponent<SphereGrip>();
            nodeGrip.attachedUpdateDelegate += new System.Action<Hand>(OnTriggerGripUpdate);
        }

        private void OnEnable()
        {
            CreatePoint(transform.position, transform.rotation);
        }

        private void OnDisable()
        {
            RemovePoint(nodeIndex);
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

        private void OnTriggerGripUpdate(Hand hand)
        {

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

            nodeIndex = nodeCounter;
            nodeCounter++;
        }
        
        private void RemovePoint(int index)
        {
            splineContainer.Spline.RemoveAt(index);
            nodeCounter--;
            nodeIndex = nodeCounter;
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