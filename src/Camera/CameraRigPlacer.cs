using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SLZ.Bonelab;

namespace NEP.MonoDirector.Cameras
{
    /// <summary>
    /// Simple camera controller. Can place down points in the world for
    /// the camera rig to follow.
    /// </summary>
    /// 
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class CameraRigPlacer : MonoBehaviour
    {
        public CameraRigPlacer(System.IntPtr ptr) : base(ptr) { }

        public static CameraRigPlacer instance;

        private int index = 0;

        private Camera _cam;
        public Camera cam
        {
            get
            {
                return _cam;
            }
        }

        private RigScreenOptions rigOptions;
        private Camera freeCam;

        private Vector3 wishDir = Vector3.zero;
        private float maxLength = 5f;
        private float currentSpeed = 5f;
        private float slowSpeed = 5f;
        private float fastSpeed = 10f;
        private float friction = 70f;
        private float delta => Time.deltaTime;

        private bool fastCamera => Input.GetKey(KeyCode.LeftShift);
        private bool lockCursor => Input.GetMouseButtonDown(1);

        private float x = 0f;
        private float y = 0f;
        private float z = 0f;

        private float m_xMov = 0f;
        private float m_yMov = 0f;
        private float m_zMov = 0f;

        private float xMouse = 0f;
        private float yMouse = 0f;
        private float zMouse = 0f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance.gameObject);
            }

            rigOptions = FindObjectOfType<RigScreenOptions>();
        }

        private void Update()
        {
            if(freeCam == null)
            {
                freeCam = rigOptions?.cam;
            }
            else
            {
                _cam = freeCam;

                MoveUpdate();

                if (Input.GetMouseButton(1))
                {
                    MouseUpdate();
                }
            }
        }

        private void MouseUpdate()
        {
            float x = Input.GetAxisRaw("Mouse X");
            float y = Input.GetAxisRaw("Mouse Y");

            bool rollCam = Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(1);

            freeCam.fieldOfView -= Input.GetAxisRaw("Mouse ScrollWheel") * 6f;

            if (rollCam)
            {
                zMouse -= x;
            }

            xMouse += x;
            yMouse -= y;

            freeCam.transform.rotation = Quaternion.Euler((Vector3.right * yMouse + Vector3.up * xMouse + Vector3.forward * zMouse) * 4f);
        }

        private void MoveUpdate()
        {
            Vector3.ClampMagnitude(wishDir, maxLength);

            int yNeg = Input.GetKey(KeyCode.Q) ? -1 : 0;
            int yPos = Input.GetKey(KeyCode.E) ? 1 : 0;

            x = Input.GetAxisRaw("Horizontal");
            y = yNeg + yPos;
            z = Input.GetAxisRaw("Vertical");

            Transform t = freeCam.transform;

            currentSpeed = fastCamera ? fastSpeed : slowSpeed;

            m_xMov += x * currentSpeed * delta;
            m_yMov += y * currentSpeed * delta;
            m_zMov += z * currentSpeed * delta;

            if(x == 0)
            {
                if(m_xMov != 0f)
                {
                    m_xMov = Mathf.MoveTowards(m_xMov, 0f, friction * Time.deltaTime);
                }
            }

            if(y == 0)
            {
                if (m_yMov != 0f)
                {
                    m_yMov = Mathf.MoveTowards(m_yMov, 0f, friction * Time.deltaTime);
                }
            }

            if(z == 0)
            {
                if (m_zMov != 0f)
                {
                    m_zMov = Mathf.MoveTowards(m_zMov, 0f, friction * Time.deltaTime);
                }
            }

            Vector3 movVec = new Vector3(m_xMov, m_yMov, m_zMov);
            movVec = Vector3.ClampMagnitude(movVec, maxLength);

            wishDir = (t.right * movVec.x) + (t.up * movVec.y) + (t.forward * movVec.z);

            t.position += wishDir * delta;
        }
    }

}