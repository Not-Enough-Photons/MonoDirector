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

namespace NEP.MonoDirector.Tools;

[RegisterTypeInIl2Cpp]
public class SceneShelf(IntPtr ptr) : MonoBehaviour(ptr)
{
    public static SceneShelf Instance { get; private set; }

    private Film m_film;
    private SceneShelfSlot[] m_slots;
    private SceneShelfSlot m_newSceneSlot;
    private SceneShelfSlot m_deleteSceneSlot;
    private SceneShelfSlot m_activeSceneSlot;

    private UIButton m_closeButton;

    private Spawnable m_reelSpawnable;

    private AudioSource m_deleteSfx;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        Transform shelf = transform.Find("Canvas/Shelf");

        m_slots = new SceneShelfSlot[shelf.childCount];

        for (int i = 0; i < shelf.childCount; i++)
        {
            Transform child = shelf.GetChild(i);
            m_slots[i] = child.GetComponent<SceneShelfSlot>();
        }

        m_newSceneSlot = transform.Find("Canvas/NewStage").GetComponent<SceneShelfSlot>();
        m_deleteSceneSlot = transform.Find("Canvas/Trash").GetComponent<SceneShelfSlot>();
        m_activeSceneSlot = transform.Find("Canvas/ActiveStage").GetComponent<SceneShelfSlot>();
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

        foreach (var slot in m_slots)
        {
            slot.OnConnected += OnReelConnected;
            slot.OnDisconnected += OnReelDisconnected;
        }

        m_newSceneSlot.OnDisconnected += OnNewReel;
        m_deleteSceneSlot.OnConnected += OnDeleteReel;
        m_activeSceneSlot.OnConnected += OnActiveReelSet;

        m_newSceneSlot.Reel.transform.position = m_newSceneSlot.transform.position;
        m_newSceneSlot.Reel.transform.rotation = m_newSceneSlot.transform.rotation;
        m_newSceneSlot.Reel.Show();

        UpdateSlotLayout();
        UpdateUtilitySockets();

        m_closeButton.OnClicked += Hide;
    }

    private void OnDisable()
    {
        foreach (var slot in m_slots)
        {
            slot.OnConnected -= OnReelConnected;
            slot.OnDisconnected -= OnReelDisconnected;
        }

        m_newSceneSlot.OnDisconnected -= OnNewReel;
        m_deleteSceneSlot.OnConnected -= OnDeleteReel;
        m_activeSceneSlot.OnConnected -= OnActiveReelSet;

        m_closeButton.OnClicked -= Hide;
    }

    public void Show()
    {
        for (int i = 0; i < m_slots.Length; i++)
            m_slots[i].Show();

        if (m_activeSceneSlot.Reel)
            m_activeSceneSlot.Reel.Show();

        m_newSceneSlot.Show();

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        for (int i = 0; i < m_slots.Length; i++)
            m_slots[i].Hide();

        if (m_activeSceneSlot.Reel)
            m_activeSceneSlot.Reel.Hide();

        m_newSceneSlot.Hide();

        gameObject.SetActive(false);
    }

    private void OnReelConnected(SceneReel reel)
    {
        if (reel == null)
            return;

        UpdateSlotLayout();
    }

    private void OnReelDisconnected(SceneReel reel)
    {
        if (reel == null)
            return;

        UpdateSlotLayout();
    }

    private void OnNewReel(SceneReel reel)
    {
        MelonCoroutines.Start(SpawnNewReel());
    }

    private void OnDeleteReel(SceneReel reel)
    {
        if (m_deleteSceneSlot.Reel.Scene == null)
        {
            m_deleteSceneSlot.Reel.Disconnect();
            m_deleteSceneSlot.Reel.Despawn();
            return;
        }

        int index = Director.ActiveFilm.Scenes.Count - 1;

        if (index <= 0)
            index = 0;

        Scene previousScene = Director.ActiveFilm.Scenes[index];
        Director.RemoveScene(m_deleteSceneSlot.Reel.Scene);
        Director.SetScene(previousScene);

        m_deleteSceneSlot.Reel.Despawn();
        m_deleteSceneSlot.Reel.SetScene(null);
        // m_deleteStageSocket.Reel.Connect(m_newStageSocket);
        m_deleteSceneSlot.Socket.Unbind();

        UpdateSlotLayout();

        m_deleteSfx.Play();
    }

    private void OnActiveReelSet(SceneReel reel)
    {
        if (reel == null)
            return;

        Director.SetScene(reel.Scene);
    }

    private IEnumerator SpawnNewReel()
    {
        yield return new
                WaitForAssetSpawn<SceneReel>(m_reelSpawnable, Vector3.zero, Quaternion.identity)
                .Then(reel =>
                {
                    reel.SetColor(Color.HSVToRGB(Random.value, 1.0f, 1.0f));
                    reel.Connect(m_newSceneSlot);
                    m_newSceneSlot.SetReel(reel);
                });
    }

    private IEnumerator SpawnReels()
    {
        for (int i = 0; i < Director.ActiveFilm.Scenes.Count; i++)
        {
            Scene scene = Director.ActiveFilm.Scenes[i];
            SceneShelfSlot slot = m_slots[i];

            yield return new
                WaitForAssetSpawn<SceneReel>(m_reelSpawnable, Vector3.zero, Quaternion.identity)
                .Then(reel =>
                {
                    reel.Hide();
                    slot.SetReel(reel);
                    reel.SetScene(scene);
                    reel.Connect(slot);
                    reel.SetColor(Color.HSVToRGB(Random.value, 1.0f, 1.0f));
                });
        }

        yield return new
            WaitForAssetSpawn<SceneReel>(m_reelSpawnable, Vector3.zero, Quaternion.identity)
            .Then(reel =>
            {
                m_newSceneSlot.Initialize();
                reel.Connect(m_newSceneSlot);
                m_newSceneSlot.SetReel(reel);
                reel.Hide();
            });
    }

    private void UpdateSlotLayout()
    {
        for (int i = 0; i < m_slots.Length; i++)
            m_slots[i].Hide();

        for (int i = 0; i < m_slots.Length && i < m_film.Scenes.Count; i++)
        {
            SceneShelfSlot slot = m_slots[i];
            slot.Show();

            if (!slot.Reel)
                continue;

            Vector3 position = slot.transform.position;
            Quaternion rotation = slot.transform.rotation;
            slot.Reel.transform.position = position;
            slot.Reel.transform.rotation = rotation;
        }

        // Set the last slot active to add new reels to the end
        m_slots[m_film.Scenes.Count].Show();
    }

    private void UpdateUtilitySockets()
    {
        // Reset the position and rotation of the new scene slot's reel
        m_newSceneSlot.Reel.transform.position = m_newSceneSlot.transform.position;
        m_newSceneSlot.Reel.transform.rotation = m_newSceneSlot.transform.rotation;

        // Reset the active scene reel position and rotation too
        if (m_activeSceneSlot.Reel)
        {
            m_activeSceneSlot.Reel.transform.position = m_activeSceneSlot.transform.position;
            m_activeSceneSlot.Reel.transform.rotation = m_activeSceneSlot.transform.rotation;
        }
    }

    private void RegisterSpawnable()
    {
        SpawnableCrateReference crateRef = new SpawnableCrateReference()
        {
            Barcode = new Barcode("NEP.MonoDirector.Spawnable.SceneReel")
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