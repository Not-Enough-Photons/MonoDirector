using Il2CppSLZ.Marrow;
using NEP.MonoDirector.Actors;
using UnityEngine;

namespace NEP.MonoDirector.UI
{
    [Obsolete("The generic MarkerManager class should be used!")]
    public sealed class PropMarker
    {
        public enum PropType
        {
            None,
            Generic,
            Gun,
            Magazine,
            Vehicle,
            NPC
        }

        public bool Active => m_gameObject.activeInHierarchy;
        public bool HasProp => m_prop != null;
        public PropType Type => m_type;

        private GameObject m_gameObject;
        private Prop m_prop;
        private Vector3 m_target;
        private Vector3 m_offset;
        private PropType m_type;

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
            if (prop == null)
                return;

            m_prop = prop;

            if (Prop.EligibleWithType<GunProp>(prop.Entity))
                m_type = PropType.Gun;
            else if (Prop.EligibleWithType<Magazine>(prop.Entity))
                m_type = PropType.Magazine;
            else if (Prop.EligibleWithType<Atv>(prop.Entity))
                m_type = PropType.Vehicle;
            else
                m_type = PropType.Generic;
            
            m_gameObject.transform.position = m_prop.transform.position;

            if (!m_prop.Entity)
            {
                Hide();
                return;
            }

            Bounds bounds = m_prop.Entity.AnchorBody.Bounds;
            SetOffset(new Vector3(0f, (bounds.center.y + bounds.extents.y) + m_markerPadding, 0f));
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

        private void UpdateIcon()
        {
            if (m_type == PropType.None)
            {
                return;
            }
        }
    }
}
