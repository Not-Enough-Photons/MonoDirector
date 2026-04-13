using Il2CppSLZ.Marrow.Interaction;
using NEP.MonoDirector.Actors;
using UnityEngine;

namespace NEP.MonoDirector.UI
{
    public sealed class PropMarker
    {
        public bool Active => m_gameObject.activeInHierarchy;
        public bool HasProp => m_prop != null;

        private GameObject m_gameObject;
        private Prop m_prop;
        private Vector3 m_target;
        private Vector3 m_offset;

        private const float m_markerPadding = 0.25f;

        public void SetTarget(Vector3 target)
        {
            m_target = target;
        }

        public void SetOffset(Vector3 offset)
        {
            m_offset = offset;
        }

        public void SetMarker(GameObject marker)
        {
            m_gameObject = marker; 
        }

        public void SetProp(Prop prop)
        {
            m_prop = prop;

            if (m_prop != null)
            {
                m_gameObject.transform.position = m_prop.transform.position;

                if (!m_prop.InteractableRigidbody)
                {
                    Hide();
                    return;
                }

                // Temp hack for 1.2.0
                if (!MarrowBody.Cache.TryGet(m_prop.InteractableRigidbody.gameObject, out MarrowBody mb))
                {
                    Hide();
                    return;
                }

                Bounds bounds = mb.Bounds;
                SetOffset(new Vector3(0f, (bounds.center.y + bounds.extents.y) + m_markerPadding, 0f));
            }
        }

        public void Update()
        {
            if (m_gameObject == null)
            {
                return;
            }

            if (m_prop == null)
            {
                return;
            }

            m_target = m_prop.transform.position;
            m_gameObject.transform.position = Vector3.Lerp(m_gameObject.transform.position, m_target + m_offset, 8f * Time.deltaTime);
        }

        public void Show()
        {
            m_gameObject.SetActive(true);
        }

        public void Hide()
        {
            m_gameObject.SetActive(false);
        }
    }
}
