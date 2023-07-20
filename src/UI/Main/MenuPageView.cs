using NEP.MonoDirector.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class MenuPageView : MonoBehaviour
    {
        public MenuPageView(System.IntPtr ptr) : base(ptr) { }

        private Button playheadButton;
        private Button actorListButton;
        private Button settingsButton;
        private Button exitButton;

        private void Awake()
        {
            playheadButton = transform.Find("Option_Playhead").GetComponent<Button>();
            actorListButton = transform.Find("Option_Actors").GetComponent<Button>();
            settingsButton = transform.Find("Option_Settings").GetComponent<Button>();
            exitButton = transform.Find("Option_Exit").GetComponent<Button>();

            playheadButton.onClick.AddListener(new System.Action(() => OnPlayheadClicked()));
            actorListButton.onClick.AddListener(new System.Action(() => OnActorListClicked()));
            settingsButton.onClick.AddListener(new System.Action(() => OnSettingsClicked()));
            exitButton.onClick.AddListener(new System.Action(() => OnExitClicked()));
        }

        public void OnPlayheadClicked()
        {
            MenuUI.Instance.SetPage("Playhead");
        }

        public void OnActorListClicked()
        {
            MenuUI.Instance.SetPage("Cast");
        }

        public void OnSettingsClicked()
        {
            MenuUI.Instance.SetPage("Settings");
        }

        public void OnExitClicked()
        {
            MenuUI.Instance.DisplayMenu(false);
        }
    }
}