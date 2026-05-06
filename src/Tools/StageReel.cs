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
    public class StageReel(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public Stage Stage => m_stage;
        public UIPlug Plug => m_plug;
        public StageShelfSocket AttachedSocket => m_attachedSocket;

        private Stage m_stage;
        private Rigidbody m_rigidbody;
        private TextMeshPro m_title;
        private StageShelfSocket m_attachedSocket;
        private StageShelfSocket m_lastConnectedSocket;
        private StageShelfSocket m_hoveredSocket;
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
            StageShelfSocket socket = collider.GetComponent<StageShelfSocket>();

            if (socket == null)
                return;

            m_hoveredSocket = socket;
        }

        private void OnTriggerExit(Collider collider)
        {
            StageShelfSocket socket = collider.GetComponent<StageShelfSocket>();

            if (socket == null)
                return;

            if (m_hoveredSocket == socket)
                m_hoveredSocket = null;
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

        public void SetStage(Stage stage)
        {
            m_stage = stage;

            if (m_stage == null)
                m_title.text = "None";
            else
                m_title.text = m_stage.Name;
        }

        public void Connect(StageShelfSocket socket)
        {
            m_attachedSocket = socket;
            m_lastConnectedSocket = m_attachedSocket;
            m_attachedSocket.SetReel(this);
            m_plug.Connect(socket.Socket);

            m_connect1.Play();
            m_connect2.Play();
        }

        public void Disconnect()
        {
            if (!m_attachedSocket || !m_attachedSocket.Socket)
                return;

            if (m_attachedSocket.IsDisconnected)
                return;

            m_attachedSocket.SetReel(null);
            m_attachedSocket = null;
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

            // Already hovering over an empty socket?
            if (m_hoveredSocket && m_hoveredSocket.Socket.Empty)
            {
                // If there's no stage already, make one.
                if (m_stage == null)
                {
                    m_stage = new Stage("Stage");
                    SetStage(m_stage);
                    Director.SetStage(m_stage);
                    Director.AddStage(m_stage);
                }

                Connect(m_hoveredSocket);
            }
            // If we let go of the reel, it should go back to the previously connected socket.
            else if (m_lastConnectedSocket)
            {
                if (m_stage == null)
                    Despawn();

                Connect(m_lastConnectedSocket);
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
