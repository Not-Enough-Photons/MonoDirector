using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI.Interaction
{
    [RegisterTypeInIl2Cpp]
    public class UISocket(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public event Action OnConnected;
        public event Action OnDisconnected;

        public bool Hovered => m_hovered;
        public bool Empty => m_empty;
        
        private Color m_normalColor = Color.white;
        private Color m_hoverColor = Color.blue;
        private Image m_image;
        
        private bool m_hovered;
        private bool m_empty = true;

        private UIPlug m_plug;

        private void Awake()
        {
            m_image = transform.Find("Outline").GetComponent<Image>();
        }

        public void HoverOver()
        {
            if (!m_empty)
                return;

            if (!m_image)
                return;
            
            m_image.CrossFadeColor(m_hoverColor, 0.1f, true, true);
            m_hovered = true;
        }

        public void HoverAway()
        {
            if (!m_image)
                return;
            
            m_image.CrossFadeColor(m_normalColor, 0.1f, true, true);
            m_hovered = false;
        }
        
        public void Bind(UIPlug plug)
        {
            if (!plug)
                return;

            if (!m_empty)
                return;
            
            m_plug = plug;
            m_plug.SetPosition(transform.position);
            m_plug.SetRotation(transform.rotation);
            m_plug.Body.isKinematic = true;
            m_empty = false;
            HoverAway();
            OnConnected?.Invoke();
        }

        public void Unbind()
        {
            if (!m_plug || !m_plug.Body)
                return;

            m_plug.Body.isKinematic = false;
            m_plug = null;
            m_empty = true;
            OnDisconnected?.Invoke();
        }
    }   
}