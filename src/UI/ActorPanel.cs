using UnityEngine;
using MelonLoader;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Actors;
using Il2CppTMPro;
using UnityEngine.UI;
using NEP.MonoDirector.State;

namespace NEP.MonoDirector.UI
{
    [RegisterTypeInIl2Cpp]
    public class ActorPanel(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private Transform m_root;

        private TextMeshProUGUI m_actorNameText;
        private Button m_recastButton;
        private Button m_deleteButton;
        private Button m_hideButton;

        private Action m_onRecastClicked;
        private Action m_onDeleteClicked;
        private Action m_onHideClicked;

        private Vector3 m_targetPosition;
        private bool m_hidden;

        private void Awake()
        {
            m_root = transform.GetChild(0);

            m_actorNameText = m_root.Find("ActorName").GetComponent<TextMeshProUGUI>();
            m_recastButton = m_root.Find("Management/Recast").GetComponent<Button>();
            m_deleteButton = m_root.Find("Management/Delete").GetComponent<Button>();
            m_hideButton = m_root.Find("Management/Hide").GetComponent<Button>();

            m_onRecastClicked = OnRecastClicked;
            m_onDeleteClicked = OnDeleteClicked;
            m_onHideClicked = OnHideClicked;

            m_root.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            Caster.OnActorSelected += OnActorSelected;
            Caster.OnActorDeselected += OnActorDeselected;
            Events.OnPlayStateSet += OnPlayStateSet;

            m_recastButton.onClick.AddListener(m_onRecastClicked);
            m_deleteButton.onClick.AddListener(m_onDeleteClicked);
            m_hideButton.onClick.AddListener(m_onHideClicked);
        }

        private void OnDisable()
        {
            Caster.OnActorSelected -= OnActorSelected;
            Caster.OnActorDeselected -= OnActorDeselected;
            Events.OnPlayStateSet -= OnPlayStateSet;

            m_recastButton.onClick.RemoveListener(m_onRecastClicked);
            m_deleteButton.onClick.RemoveListener(m_onDeleteClicked);
            m_hideButton.onClick.RemoveListener(m_onHideClicked);
        }

        private void Update()
        {
            Vector3 rotation = Quaternion.LookRotation(Constants.RigManager.physicsRig.m_head.position - transform.position).eulerAngles;
            transform.position = Vector3.Lerp(transform.position, m_targetPosition, 8f * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }

        private void OnActorSelected(Actor actor)
        {
            m_root.gameObject.SetActive(true);

            m_targetPosition = actor.ActorBody.Chest.transform.position + Vector3.right;

            m_actorNameText.text = actor.ActorName;
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
            Director.ActiveStage.RemoveActor(Caster.SelectedActor);
            m_root.gameObject.SetActive(false);
        }

        private void OnHideClicked()
        {
            m_hidden = !m_hidden;
            Caster.SelectedActor.SetHidden(m_hidden);
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
