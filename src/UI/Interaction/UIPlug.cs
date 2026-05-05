using MelonLoader;
using UnityEngine;

namespace NEP.MonoDirector.UI.Interaction
{
    [RegisterTypeInIl2Cpp]
    public class UIPlug(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public UISocket HoveredSocket => m_hoveredSocket;
        public Rigidbody Body => m_rigidbody;

        private Rigidbody m_rigidbody;
        
        private UISocket m_socket;
        private UISocket m_hoveredSocket;
        
        private void Awake()
        {
            m_rigidbody = transform.parent.GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            UISocket socket = other.GetComponent<UISocket>();

            if (socket == null)
                return;

            m_hoveredSocket = socket;

            if (m_hoveredSocket.Empty)
                m_hoveredSocket.HoverOver();
        }

        private void OnTriggerExit(Collider other)
        {
            UISocket socket = other.GetComponent<UISocket>();

            if (socket == null)
                return;

            if (m_hoveredSocket != socket)
                return;
            
            m_hoveredSocket.HoverAway();
            m_hoveredSocket = null;
        }

        public void Parent(UISocket socket)
        {
            if (!socket)
                return;

            m_rigidbody.transform.SetParent(socket.transform);
            m_rigidbody.transform.localPosition = Vector3.zero;
            m_rigidbody.transform.localRotation = Quaternion.identity;
        }

        public void Unparent()
        {
            m_rigidbody.transform.SetParent(null);
            m_rigidbody.transform.localPosition = Vector3.zero;
            m_rigidbody.transform.localRotation = Quaternion.identity;
        }

        public void SetPosition(Vector3 position)
        {
            m_rigidbody.transform.position = position;
        }

        public void SetRotation(Quaternion rotation)
        {
            m_rigidbody.transform.rotation = rotation;
        }

        public void Connect(UISocket socket)
        {
            if (!socket)
                return;
            
            m_socket = socket;
            m_socket.Bind(this);
        }

        public void Disconnect()
        {
            if (m_socket) 
                m_socket.Unbind();
            
            m_socket = null;
        }
    }
}