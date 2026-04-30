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

        private TextMeshProUGUI m_actorNameText;
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

        private void Awake()
        {
            m_root = transform.GetChild(0);

            m_actorNameText = m_root.Find("ActorName").GetComponent<TextMeshProUGUI>();
            m_recastButton = m_root.Find("Management/Recast").GetComponent<UIButton>();
            m_deleteButton = m_root.Find("Management/Delete").GetComponent<UIButton>();
            m_hideButton = m_root.Find("Management/Hide").GetComponent<UIButton>();
            m_closeButton = m_root.Find("Close").GetComponent<UIButton>();
            
            m_onRecastClicked = OnRecastClicked;
            m_onDeleteClicked = OnDeleteClicked;
            m_onHideClicked = OnHideClicked;
            m_onCloseClicked = OnCloseClicked;

            //m_root.gameObject.SetActive(false);
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
            gameObject.SetActive(true);

            m_targetPosition = actor.ActorBody.Chest.transform.position + Vector3.right;

            m_actorNameText.text = actor.ActorName;
        }

        private void OnActorDeselected(Actor actor)
        {
            gameObject.SetActive(false);
        }

        private void OnRecastClicked()
        {
            Caster.RecastActor(Caster.SelectedActor);
            gameObject.SetActive(false);
        }

        private void OnDeleteClicked()
        {
            Caster.UncastActor(Caster.SelectedActor);
            // TODO: Move this into Caster or something
            Director.ActiveStage.RemoveActor(Caster.SelectedActor);
            gameObject.SetActive(false);
        }

        private void OnHideClicked()
        {
            m_hidden = !m_hidden;
            Caster.SelectedActor.SetHidden(m_hidden);
        }

        private void OnCloseClicked()
        {
            gameObject.SetActive(false);
        }

        private void OnPlayStateSet(PlayState state)
        {
            if (Caster.SelectedActor == null)
                return;

            if (state != PlayState.Stopped)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
        }
    }
}
