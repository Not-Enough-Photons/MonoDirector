using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.Warehouse;

using MelonLoader;

using NEP.MonoDirector.Core;

using System.Collections;
using NEP.MonoDirector.UI.Interaction;
using UnityEngine;
using NEP.MonoDirector.Yielding;

using Random = UnityEngine.Random;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class StageShelf(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public static StageShelf Instance { get; private set; }

        private Film m_film;
        private StageShelfSocket[] m_sockets;
        private StageShelfSocket m_newStageSocket;
        private StageShelfSocket m_deleteStageSocket;
        private StageShelfSocket m_activeStageSocket;

        private UIButton m_closeButton;
        
        private Spawnable m_reelSpawnable;

        private AudioSource m_deleteSfx;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            
            Transform shelf = transform.Find("Canvas/Shelf");

            m_sockets = new StageShelfSocket[shelf.childCount];

            for (int i = 0; i < shelf.childCount; i++)
            {
                Transform child = shelf.GetChild(i);
                m_sockets[i] = child.GetComponent<StageShelfSocket>();
            }

            m_newStageSocket = transform.Find("Canvas/NewStage").GetComponent<StageShelfSocket>();
            m_deleteStageSocket = transform.Find("Canvas/Trash").GetComponent<StageShelfSocket>();
            m_activeStageSocket = transform.Find("Canvas/ActiveStage").GetComponent<StageShelfSocket>();
            m_deleteSfx = transform.Find("Canvas/Trash/DeleteSFX").GetComponent<AudioSource>();

            m_closeButton = transform.Find("Canvas/CloseButton").GetComponent<UIButton>();

            RegisterSpawnable();

            MelonCoroutines.Start(SpawnReels());

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            m_film = Director.ActiveFilm;

            Transform playerChest = BoneLib.Player.PhysicsRig.m_chest;
            transform.position = playerChest.position + playerChest.forward * 2f;
            // Calculate look at
            Vector3 lookRotation = Quaternion.LookRotation(playerChest.position - transform.position).eulerAngles;
            Quaternion yRotation = Quaternion.Euler(0f, lookRotation.y, 0f);
            transform.rotation = yRotation;

            foreach (var socket in m_sockets)
            {
                socket.OnConnected += OnReelConnected;
                socket.OnDisconnected += OnReelDisconnected;
            }

            m_newStageSocket.OnDisconnected += OnNewReel;
            m_deleteStageSocket.OnConnected += OnDeleteReel;
            m_activeStageSocket.OnConnected += OnActiveReelSet;

            m_newStageSocket.Reel.transform.position = m_newStageSocket.transform.position;
            m_newStageSocket.Reel.transform.rotation = m_newStageSocket.transform.rotation;
            m_newStageSocket.Reel.Show();
            
            UpdateSocketLayout();
            UpdateUtilitySockets();
            
            m_closeButton.OnClicked += Hide;
        }

        private void OnDisable()
        {
            foreach (var socket in m_sockets)
            {
                socket.OnConnected -= OnReelConnected;
                socket.OnDisconnected -= OnReelDisconnected;
            }

            m_newStageSocket.OnDisconnected -= OnNewReel;
            m_deleteStageSocket.OnConnected -= OnDeleteReel;
            m_activeStageSocket.OnConnected -= OnActiveReelSet;
            
            m_closeButton.OnClicked -= Hide;
        }

        public void Show()
        {
            for (int i = 0; i < m_sockets.Length; i++)
                m_sockets[i].Show();

            if (m_activeStageSocket.Reel)
                m_activeStageSocket.Reel.Show();
            
            m_newStageSocket.Show();
            
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            for (int i = 0; i < m_sockets.Length; i++)
                m_sockets[i].Hide();

            if (m_activeStageSocket.Reel)
                m_activeStageSocket.Reel.Hide();
            
            m_newStageSocket.Hide();
            
            gameObject.SetActive(false);
        }

        private void OnReelConnected(StageReel reel)
        {
            if (reel == null)
                return;

            UpdateSocketLayout();
        }

        private void OnReelDisconnected(StageReel reel)
        {
            if (reel == null)
                return;

            UpdateSocketLayout();
        }

        private void OnNewReel(StageReel reel)
        {
            MelonCoroutines.Start(SpawnNewReel());
        }

        private void OnDeleteReel(StageReel reel)
        {
            if (m_deleteStageSocket.Reel.Stage == null)
            {
                m_deleteStageSocket.Reel.Disconnect();
                m_deleteStageSocket.Reel.Despawn();
                return;
            }

            int index = Director.ActiveFilm.Stages.Count - 1;

            if (index <= 0)
                index = 0;
            
            Stage previousStage = Director.ActiveFilm.Stages[index];
            Director.RemoveStage(m_deleteStageSocket.Reel.Stage);
            Director.SetStage(previousStage);

            m_deleteStageSocket.Reel.Despawn();
            m_deleteStageSocket.Reel.SetStage(null);
            // m_deleteStageSocket.Reel.Connect(m_newStageSocket);
            m_deleteStageSocket.Socket.Unbind();
            
            UpdateSocketLayout();

            m_deleteSfx.Play();
        }

        private void OnActiveReelSet(StageReel reel)
        {
            if (reel == null)
                return;

            Director.SetStage(reel.Stage);
        }

        private IEnumerator SpawnNewReel()
        {
            yield return new
                    WaitForAssetSpawn<StageReel>(m_reelSpawnable, Vector3.zero, Quaternion.identity)
                    .Then(reel =>
                    {
                        reel.SetColor(Color.HSVToRGB(Random.value, 1.0f, 1.0f));
                        reel.Connect(m_newStageSocket);
                        m_newStageSocket.SetReel(reel);
                    });
        }

        private IEnumerator SpawnReels()
        {
            for (int i = 0; i < Director.ActiveFilm.Stages.Count; i++)
            {
                Stage stage = Director.ActiveFilm.Stages[i];
                StageShelfSocket socket = m_sockets[i];
                
                yield return new 
                    WaitForAssetSpawn<StageReel>(m_reelSpawnable, Vector3.zero, Quaternion.identity) 
                    .Then(reel =>
                    {
                        reel.Hide();
                        socket.SetReel(reel);
                        reel.SetStage(stage);
                        reel.Connect(socket);
                        reel.SetColor(Color.HSVToRGB(Random.value, 1.0f, 1.0f));
                    });
            }

            yield return new
                WaitForAssetSpawn<StageReel>(m_reelSpawnable, Vector3.zero, Quaternion.identity)
                .Then(reel =>
                {
                    // this does fuck-all because of the Awake function not getting called in time
                    // on this instance of StageShelfSocket
                    m_newStageSocket.Initialize();
                    reel.Connect(m_newStageSocket);
                    m_newStageSocket.SetReel(reel);
                    reel.Hide();
                });
        }

        private void UpdateSocketLayout()
        {
            for (int i = 0; i < m_sockets.Length; i++)
                m_sockets[i].Hide();
            
            for (int i = 0; i < m_sockets.Length && i < m_film.Stages.Count; i++)
            {
                StageShelfSocket socket = m_sockets[i];
                socket.Show();

                if (!socket.Reel)
                    continue;

                Vector3 position = socket.transform.position;
                Quaternion rotation = socket.transform.rotation;
                socket.Reel.transform.position = position;
                socket.Reel.transform.rotation = rotation;
            }

            // Set the last socket active to add new reels to the end
            m_sockets[m_film.Stages.Count].Show();
        }
        
        private void UpdateUtilitySockets()
        {
            // Reset the position and rotation of the new stage socket's reel
            m_newStageSocket.Reel.transform.position = m_newStageSocket.transform.position;
            m_newStageSocket.Reel.transform.rotation = m_newStageSocket.transform.rotation;
            
            // Reset the active stage reel position and rotation too
            if (m_activeStageSocket.Reel)
            {
                m_activeStageSocket.Reel.transform.position = m_activeStageSocket.transform.position;
                m_activeStageSocket.Reel.transform.rotation = m_activeStageSocket.transform.rotation;
            }
        }

        private void RegisterSpawnable()
        {
            SpawnableCrateReference crateRef = new SpawnableCrateReference()
            {
                Barcode = new Barcode("NEP.MonoDirector.Spawnable.StageReel")
            };

            m_reelSpawnable = new Spawnable()
            {
                crateRef = crateRef
            };

            // TODO: Clean up this very messy code
            // A lot of this can be refactored into dedicated spawn functions
            AssetSpawner.Register(m_reelSpawnable);
        }
    }
}
