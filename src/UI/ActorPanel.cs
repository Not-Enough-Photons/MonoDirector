using UnityEngine;
using MelonLoader;
using Il2CppTMPro;
using UnityEngine.UI;

using NEP.MonoDirector.Core;
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.State;
using NEP.MonoDirector.UI.Interaction;

namespace NEP.MonoDirector.UI
{
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
            
            m_root.gameObject.SetActive(false);
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
            
            Vector3 rotation = Quaternion.LookRotation(Constants.RigManager.physicsRig.m_head.position - transform.position).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
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

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, m_targetPosition, 8f * Time.deltaTime);
        }

        private void OnActorSelected(Actor actor)
        {
            m_root.gameObject.SetActive(true);

            m_targetPosition = actor.ActorBody.Chest.transform.position + Vector3.right;

            m_actorNameText.text = $"Actor Settings - {actor.ActorName}";
        }

        private void OnActorDeselected(Actor actor)
        {
            m_root.gameObject.SetActive(false);
        }

        private void OnRecastClicked()
        {
            Caster.RecastActor(Caster.SelectedActor);
            m_root.gameObject.SetActive(false);
        }

        private void OnDeleteClicked()
        {
            Caster.UncastActor(Caster.SelectedActor);
            // TODO: Move this into Caster or something
            Director.ActiveScene.RemoveActor(Caster.SelectedActor);
            m_root.gameObject.SetActive(false);
        }

        private void OnHideClicked()
        {
            m_hidden = !m_hidden;
            Caster.SelectedActor.SetHidden(m_hidden);
            m_hiddenCheckmark.SetActive(m_hidden);
        }

        private void OnCloseClicked()
        {
            m_root.gameObject.SetActive(false);
        }

        private void OnPlayStateSet(PlayState state)
        {
            if (Caster.SelectedActor == null)
                return;

            if (state != PlayState.Stopped)
            {
                m_root.gameObject.SetActive(false);
                return;
            }

            m_root.gameObject.SetActive(true);
        }
    }
}
