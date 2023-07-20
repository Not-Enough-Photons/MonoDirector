using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SettingsPageView : MonoBehaviour
    {
        public SettingsPageView(System.IntPtr ptr) : base(ptr) { }

        private Button option_Audio;
        private Button option_Camera;
        private Button option_World;
        private Button option_Debug;
        private Button option_Credits;

        private GameObject page_Audio => pagesGroup.Find("Page_Audio").gameObject;
        private GameObject page_Camera => pagesGroup.Find("Page_Camera").gameObject;
        private GameObject page_Credits => pagesGroup.Find("Page_Credits").gameObject;

        private Transform optionsGroup => transform.Find("Options");
        private Transform pagesGroup => transform.Find("Pages");

        private void Awake()
        {
            option_Audio = optionsGroup.Find("Option_Audio").GetComponent<Button>();
            option_Camera = optionsGroup.Find("Option_Camera").GetComponent<Button>();
            option_World = optionsGroup.Find("Option_World").GetComponent<Button>();
            option_Debug = optionsGroup.Find("Option_Debug").GetComponent<Button>();
            option_Credits = optionsGroup.Find("Option_Credits").GetComponent<Button>();

            optionsGroup.gameObject.SetActive(true);
        }

        private void Start()
        {
            option_Audio.onClick.AddListener(new System.Action(() => OnOptionAudioClicked()));
            option_Camera.onClick.AddListener(new System.Action(() => OnOptionCameraClicked()));
            option_World.onClick.AddListener(new System.Action(() => OnOptionWorldClicked()));
            option_Debug.onClick.AddListener(new System.Action(() => OnOptionDebugClicked()));
            option_Credits.onClick.AddListener(new System.Action(() => OnOptionCreditsClicked()));
        }

        private void OnOptionAudioClicked()
        {
            MenuUI.Instance.SetPage("Settings/Option");

            optionsGroup.gameObject.SetActive(false);

            page_Audio.gameObject.SetActive(true);
            page_Camera.gameObject.SetActive(false);
            page_Credits.gameObject.SetActive(false);
        }

        private void OnOptionCameraClicked()
        {
            MenuUI.Instance.SetPage("Settings/Option");

            optionsGroup.gameObject.SetActive(false);

            page_Audio.gameObject.SetActive(false);
            page_Camera.gameObject.SetActive(true);
            page_Credits.gameObject.SetActive(false);
        }

        private void OnOptionWorldClicked()
        {

        }

        private void OnOptionDebugClicked()
        {

        }

        private void OnOptionCreditsClicked()
        {
            MenuUI.Instance.SetPage("Settings/Option");

            optionsGroup.gameObject.SetActive(false);

            page_Audio.gameObject.SetActive(false);
            page_Camera.gameObject.SetActive(false);
            page_Credits.gameObject.SetActive(true);
        }
    }
}