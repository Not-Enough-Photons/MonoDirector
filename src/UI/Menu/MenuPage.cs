using UnityEngine;

using MelonLoader;

using NEP.MonoDirector.Tools;
using NEP.MonoDirector.UI.Interaction;

namespace NEP.MonoDirector.UI.Menus
{
    [RegisterTypeInIl2Cpp]
    public class MenuPage(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private Menu m_menu;

        private UIButton m_playbackButton;
        private UIButton m_actorsButton;
        private UIButton m_stagesButton;
        private UIButton m_settingsButton;
        private UIButton m_exitButton;

        private void Awake()
        {
            m_menu = transform.root.GetComponent<Menu>();

            if (!m_menu)
                throw new NullReferenceException("Menu is null!");
            
            m_playbackButton = transform.Find("Option_Playback").GetComponent<UIButton>();
            m_actorsButton = transform.Find("Option_Actors").GetComponent<UIButton>();
            m_stagesButton = transform.Find("Option_Stages").GetComponent<UIButton>();
            m_settingsButton = transform.Find("Option_Settings").GetComponent<UIButton>();
            m_exitButton = transform.Find("Option_Exit").GetComponent<UIButton>();
        }

        private void OnEnable()
        {
            m_playbackButton.OnClicked += OnPlaybackButtonClicked;
            m_actorsButton.OnClicked += OnActorsButtonClicked;
            m_stagesButton.OnClicked += OnStagesButtonClicked;
            m_settingsButton.OnClicked += OnSettingsButtonClicked;
            m_exitButton.OnClicked += OnExitButtonClicked;
            
        }

        private void OnDisable()
        {
            m_playbackButton.OnClicked -= OnPlaybackButtonClicked;
            m_actorsButton.OnClicked -= OnActorsButtonClicked;
            m_stagesButton.OnClicked -= OnStagesButtonClicked;
            m_settingsButton.OnClicked -= OnSettingsButtonClicked;
            m_exitButton.OnClicked -= OnExitButtonClicked;
        }
        
        private void OnPlaybackButtonClicked()
        {
            m_menu.GoToPage("Playback");
        }
        
        private void OnActorsButtonClicked()
        {
        }
        
        private void OnStagesButtonClicked()
        {
            StageShelf.Instance.Hide();
            StageShelf.Instance.Show();
            m_menu.Hide();
            MenuBootstrap.HideGameMenu();
        }

        private void OnSettingsButtonClicked()
        {
            MenuBootstrap.OpenPage(MDBoneMenu.SettingsPage);
            m_menu.Hide();
        }

        private void OnExitButtonClicked()
        {
            m_menu.Hide();
        }
    }
}
