using UnityEngine;
using MelonLoader;

using UnityEngine.UI;

using NEP.MonoDirector.UI.Interaction;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class SceneShelfSlot(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public UISocket Socket => m_socket;
        public Rigidbody Body { get => m_body; }
        public SceneReel Reel { get => m_reel; }
        public int Index => transform.GetSiblingIndex();
        public bool IsSpawner { get => m_isSpawner; }
        public bool IsDisconnected { get => m_isDisconnected; }

        public event Action<SceneReel> OnConnected;
        public event Action<SceneReel> OnDisconnected;

        private SceneReel m_reel;
        private SceneReel m_hoveredReel;
        
        private Rigidbody m_body;
        private bool m_isSpawner;
        private bool m_isDisconnected;

        private UISocket m_socket;
        
        private void Awake()
        {
            m_body = GetComponent<Rigidbody>();
            Initialize();
            m_isSpawner = false;
        }

        private void OnEnable()
        {
            m_socket.OnConnected += Connect;
            m_socket.OnDisconnected += Disconnect;
        }

        private void OnDisable()
        {
            m_socket.OnConnected -= Connect;
            m_socket.OnDisconnected -= Disconnect;
        }

        public void Initialize()
        {
            if (!m_socket)
                m_socket = GetComponent<UISocket>();
        }

        public void SetReel(SceneReel reel)
        {
            if (reel == null)
            {
                m_reel = null;
                return;
            }

            m_reel = reel;
        }

        public void Connect()
        {
            m_isDisconnected = false;

            OnConnected?.Invoke(m_reel);
        }

        public void Disconnect()
        {
            m_isDisconnected = true;

            OnDisconnected?.Invoke(m_reel);
        }

        public void Show()
        {
            if (m_reel && m_reel.Scene != null)
                m_reel.Show();
            
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (m_reel)
                m_reel.Hide();
            
            gameObject.SetActive(false);
        }
    }
}
