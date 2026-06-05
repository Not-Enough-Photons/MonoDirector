using UnityEngine;
using MelonLoader;
using Il2CppTMPro;
using UnityEngine.UI;

using NEP.MonoDirector.Core;
using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.State;
using NEP.MonoDirector.UI.Interaction;

namespace NEP.MonoDirector.UI;

[RegisterTypeInIl2Cpp]
public class ActorPanel(IntPtr ptr) : MonoBehaviour(ptr)
{
    private Transform m_root;

    private TextMeshPro m_actorNameText;
    private UIButton m_recastButton;
    private UIButton m_deleteButton;
    private UIButton m_hideButton;
    private UIButton m_closeButton;

    private Action m_onRecastClicked;
    private Action m_onDeleteClicked;
    private Action m_onHideClicked;
    private Action m_onCloseClicked;

    private Vector3 m_targetPosition;
    private bool m_hidden;

    private GameObject m_hiddenCheckmark;

    private float m_lookAtDistance = 1000f;

    private void Awake()
    {
        m_root = transform.GetChild(0);

        m_actorNameText = m_root.Find("Data/Title").GetComponent<TextMeshPro>();
        m_recastButton = m_root.Find("Data/Options/OptionsGroup/RecastButton").GetComponent<UIButton>();
        m_deleteButton = m_root.Find("Data/Options/OptionsGroup/DeleteButton").GetComponent<UIButton>();
        m_hideButton = m_root.Find("Data/Options/OptionsGroup/HideControl").GetComponent<UIButton>();
        m_closeButton = m_root.Find("Data/Exit").GetComponent<UIButton>();
        m_hiddenCheckmark = m_hideButton.transform.Find("Toggle/Checkmark").gameObject;
        
        m_onRecastClicked = OnRecastClicked;
        m_onDeleteClicked = OnDeleteClicked;
        m_onHideClicked = OnHideClicked;
        m_onCloseClicked = OnCloseClicked;

        Hide();
    }

    private void OnEnable()
    {
        Caster.OnActorSelected += OnActorSelected;
        Caster.OnActorDeselected += OnActorDeselected;
        Events.OnPlayStateSet += OnPlayStateSet;

        m_recastButton.OnClicked += m_onRecastClicked;
        m_deleteButton.OnClicked += m_onDeleteClicked;
        m_hideButton.OnClicked += m_onHideClicked;
        m_closeButton.OnClicked += m_onCloseClicked;
    }

    private void OnDisable()
    {
        Caster.OnActorSelected -= OnActorSelected;
        Caster.OnActorDeselected -= OnActorDeselected;
        Events.OnPlayStateSet -= OnPlayStateSet;

        m_recastButton.OnClicked -= m_onRecastClicked;
        m_deleteButton.OnClicked -= m_onDeleteClicked;
        m_hideButton.OnClicked -= m_onHideClicked;
        m_closeButton.OnClicked -= m_onCloseClicked;
    }

    private void FacePlayer()
    {
        Transform playerChest = BoneLib.Player.PhysicsRig.m_chest;
        Vector3 lookRotation = Quaternion.LookRotation(playerChest.position - transform.position).eulerAngles;
        Quaternion yRotation = Quaternion.Euler(0f, lookRotation.y + 180f, 0f);
        transform.rotation = yRotation;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, m_targetPosition, 8f * Time.deltaTime);
        FacePlayer();
    }

    private void OnActorSelected(Actor actor)
    {
        int actorCount = Caster.SelectedActors.Count;
        Show();
        
        if (actorCount == 1)
        {
            m_targetPosition = actor.ActorBody.Chest.transform.position + Vector3.right;
            SetTitle($"Actor Settings - {actor.ActorName}");
            m_recastButton.gameObject.SetActive(true);
        }
        else if (actorCount > 1)
        {
            SetTitle($"Selected ({actorCount}) Actors");
            m_recastButton.gameObject.SetActive(false);
        }
    }

    private void OnActorDeselected(Actor actor)
    {
        int actorCount = Caster.SelectedActors.Count;
        
        if (actorCount == 1)
        {
            m_targetPosition = actor.ActorBody.Chest.transform.position + Vector3.right;
            SetTitle($"Actor Settings - {actor.ActorName}");
            m_recastButton.gameObject.SetActive(true);
        }
        else if (actorCount > 1)
        {
            SetTitle($"Selected ({actorCount}) Actors");
            m_recastButton.gameObject.SetActive(false);
        }
        else if (actorCount == 0) // No actors to show, so hide it
            Hide();
        
    }

    private void OnRecastClicked()
    {
        if (Caster.SelectedActors.Count != 1)
            return;
        
        // Only do it for one actor,
        // it would complicate things if that wasn't true
        Director.Recast(Caster.SelectedActors[0]);
        Hide();
    }

    private void OnDeleteClicked()
    {
        var selectedActors = Caster.SelectedActors.ToList();
        
        foreach (var actor in selectedActors)
            Director.RemoveActor(actor);

        Hide();
    }

    private void OnHideClicked()
    {
        m_hidden = !m_hidden;
        
        foreach (var actor in Caster.SelectedActors)
            actor.SetHidden(m_hidden);
        
        m_hiddenCheckmark.SetActive(m_hidden);
    }

    private void OnCloseClicked()
    {
        m_root.gameObject.SetActive(false);
    }

    private void OnPlayStateSet(PlayState state)
    {
        if (Caster.SelectedActors.Count == 0)
            return;
        
        if (state != PlayState.Stopped)
        {
            Hide();
            return;
        }

        Show();
    }

    private void SetTitle(string title)
    {
        m_actorNameText.text = title;
    }

    private void Show() => m_root.gameObject.SetActive(true);
    
    private void Hide() => m_root.gameObject.SetActive(false);
}
