using NEP.MonoDirector.UI.Interaction;

using BoneLib;

using MelonLoader;
using NEP.MonoDirector.Tools;
using UnityEngine;

namespace NEP.MonoDirector.UI.Menus
{
    [RegisterTypeInIl2Cpp]
    public class Menu(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public static Menu Instance { get; private set; }
        
        private UIButton m_playbackButton;
        private UIButton m_actorsButton;
        private UIButton m_stagesButton;
        private UIButton m_settingsButton;
        private UIButton m_exitButton;
        private UIButton m_closeButton;
        
        private void Awake()
        {
            Instance = this;

            m_playbackButton = transform.Find("Body/Pages/Menu/Option_Playback").GetComponent<UIButton>();
            m_actorsButton = transform.Find("Body/Pages/Menu/Option_Actors").GetComponent<UIButton>();
            m_stagesButton = transform.Find("Body/Pages/Menu/Option_Stages").GetComponent<UIButton>();
            m_settingsButton = transform.Find("Body/Pages/Menu/Option_Settings").GetComponent<UIButton>();
            m_exitButton = transform.Find("Body/Pages/Menu/Option_Exit").GetComponent<UIButton>();
            m_closeButton = transform.Find("Header/Button").GetComponent<UIButton>();
        }

        private void OnEnable()
        {
            Transform playerChest = BoneLib.Player.PhysicsRig.m_chest;
            transform.position = playerChest.position + playerChest.forward;
            // Calculate look at
            Vector3 lookRotation = Quaternion.LookRotation(playerChest.position - transform.position).eulerAngles;
            Quaternion yRotation = Quaternion.Euler(0f, lookRotation.y + 180f, 0f);
            transform.rotation = yRotation;

            m_playbackButton.OnClicked += OnPlaybackButtonClicked;
            m_actorsButton.OnClicked += OnActorsButtonClicked;
            m_stagesButton.OnClicked += OnStagesButtonClicked;
            m_settingsButton.OnClicked += OnSettingsButtonClicked;
            m_exitButton.OnClicked += OnExitButtonClicked;
            m_closeButton.OnClicked += OnExitButtonClicked;
        }

        private void OnDisable()
        {
            m_playbackButton.OnClicked -= OnPlaybackButtonClicked;
            m_actorsButton.OnClicked -= OnActorsButtonClicked;
            m_stagesButton.OnClicked -= OnStagesButtonClicked;
            m_settingsButton.OnClicked -= OnSettingsButtonClicked;
            m_exitButton.OnClicked -= OnExitButtonClicked;
            m_closeButton.OnClicked -= OnExitButtonClicked;
        }

        public void Hide() => gameObject.SetActive(false);
        public void Show() => gameObject.SetActive(true);

        private void OnPlaybackButtonClicked()
        {
            MenuBootstrap.OpenPage(MDBoneMenu.PlaybackPage);
            Hide();
        }
        
        private void OnActorsButtonClicked()
        {
        }
        
        private void OnStagesButtonClicked()
        {
            StageShelf.Instance.Hide();
            StageShelf.Instance.Show();
            Hide();
            MenuBootstrap.HideBoneMenu();
        }

        private void OnSettingsButtonClicked()
        {
            MenuBootstrap.OpenPage(MDBoneMenu.SettingsPage);
            Hide();
        }

        private void OnExitButtonClicked()
        {
            Hide();
        }
    }
}
