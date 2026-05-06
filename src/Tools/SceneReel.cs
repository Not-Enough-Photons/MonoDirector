using UnityEngine;
using MelonLoader;
using Il2CppTMPro;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Pool;

using NEP.MonoDirector.Core;
using NEP.MonoDirector.UI.Interaction;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class SceneReel(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public Scene Scene => m_scene;
        public UIPlug Plug => m_plug;
        public SceneShelfSlot AttachedSlot => m_attachedSlot;

        private Scene m_scene;
        private Rigidbody m_rigidbody;
        private TextMeshPro m_title;
        private SceneShelfSlot m_attachedSlot;
        private SceneShelfSlot m_lastSlot;
        private SceneShelfSlot m_hoveredSlot;
        private Grip m_grip;
        private Poolee m_poolee;
        private MeshRenderer m_reelMesh;

        private AudioSource m_connect1;
        private AudioSource m_connect2;
        private AudioSource m_disconnect1;
        private AudioSource m_disconnect2;

        private Action<Hand> m_onHandAttached;
        private Action<Hand> m_onHandReleased;

        private UIPlug m_plug;
        
        private bool m_hackDespawnFlag;

        private void Awake()
        {
            m_plug = GetComponent<UIPlug>() ?? transform.Find("Plug").GetComponent<UIPlug>();
            m_title = transform.Find("Text").GetComponent<TextMeshPro>();
            m_grip = transform.Find("Grip").GetComponent<Grip>();
            m_poolee = GetComponent<Poolee>();
            m_rigidbody = GetComponent<Rigidbody>();

            m_connect1 = transform.Find("SFX/Connect 1").GetComponent<AudioSource>();
            m_connect2 = transform.Find("SFX/Connect 2").GetComponent<AudioSource>();
            m_disconnect1 = transform.Find("SFX/Disconnect 1").GetComponent<AudioSource>();
            m_disconnect2 = transform.Find("SFX/Disconnect 2").GetComponent<AudioSource>();

            m_reelMesh = transform.Find("Art/Strip").GetComponent<MeshRenderer>();
            
            m_onHandAttached = OnHandAttached;
            m_onHandReleased = OnHandReleased;

            m_rigidbody.isKinematic = true;

            m_grip.attachedHandDelegate += m_onHandAttached;
            m_grip.detachedHandDelegate += m_onHandReleased;

            m_connect1.playOnAwake = false;
            m_connect2.playOnAwake = false;
            m_disconnect1.playOnAwake = false;
            m_disconnect2.playOnAwake = false;
        }

        protected void OnDestroy()
        {
            m_grip.attachedHandDelegate -= m_onHandAttached;
            m_grip.detachedHandDelegate -= m_onHandReleased;
        }

        private void Update()
        {
            if (m_hackDespawnFlag)
            {
                m_poolee.Despawn();
                m_hackDespawnFlag = false;
            }
        }
        
        private void OnTriggerEnter(Collider collider)
        {
            SceneShelfSlot socket = collider.GetComponent<SceneShelfSlot>();

            if (socket == null)
                return;

            m_hoveredSlot = socket;
        }

        private void OnTriggerExit(Collider collider)
        {
            SceneShelfSlot socket = collider.GetComponent<SceneShelfSlot>();

            if (socket == null)
                return;

            if (m_hoveredSlot == socket)
                m_hoveredSlot = null;
        }

        public void Despawn()
        {
            // Directly despawning a poolee while in your hands triggers a stack overflow.
            // This is because Poolee.Despawn invokes Grip.detachedHandDelegate.
            // Anyone who calls Poolee.Despawn inside of code that Grip.detachedHandDelegate is subscribed to, will trigger a stack overflow.
            // To get around this, we'll just set a flag to true and check it in the Update loop for despawning.
            // I really wish I didn't have to do this.
            m_hackDespawnFlag = true;
        }

        public void SetScene(Scene scene)
        {
            m_scene = scene;

            if (m_scene == null)
                m_title.text = "None";
            else
                m_title.text = m_scene.Name;
        }

        public void Connect(SceneShelfSlot slot)
        {
            m_attachedSlot = slot;
            m_lastSlot = m_attachedSlot;
            m_attachedSlot.SetReel(this);
            m_plug.Connect(slot.Socket);

            m_connect1.Play();
            m_connect2.Play();
        }

        public void Disconnect()
        {
            if (!m_attachedSlot || !m_attachedSlot.Socket)
                return;

            if (m_attachedSlot.IsDisconnected)
                return;

            m_attachedSlot.SetReel(null);
            m_attachedSlot = null;
            m_plug.Disconnect();

            m_disconnect1.Play();
            m_disconnect2.Play();
        }

        public void SetColor(Color color)
        {
            m_reelMesh.material.color = color;
        }

        private void OnHandAttached(Hand hand)
        {
            if (m_grip.attachedHands.Count > 1)
                return;

            Disconnect();
        }

        private void OnHandReleased(Hand hand)
        {
            if (m_grip.attachedHands.Count > 1)
                return;

            // Already hovering over an empty slot?
            if (m_hoveredSlot && m_hoveredSlot.Socket.Empty)
            {
                // If there's no scene already, make one.
                if (m_scene == null)
                {
                    m_scene = new Scene("Scene");
                    SetScene(m_scene);
                    Director.SetScene(m_scene);
                    Director.AddScene(m_scene);
                }

                Connect(m_hoveredSlot);
            }
            // If we let go of the reel, it should go back to the previously connected slot.
            else if (m_lastSlot)
            {
                if (m_scene == null)
                    Despawn();

                Connect(m_lastSlot);
            }
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
