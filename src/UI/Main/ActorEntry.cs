using NEP.MonoDirector.Actors;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorEntry : MonoBehaviour
    {
        public ActorEntry(System.IntPtr ptr) : base(ptr) { }

        public RawImage avatarImage;
        public TextMeshProUGUI avatarNameText;
        private Button button;

        public Actor actorData;

        public void Start()
        {
            avatarImage = transform.Find("Avatar").GetComponent<RawImage>();
            avatarNameText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            button = GetComponent<Button>();

            button.targetGraphic = avatarImage;
            button.onClick.AddListener(new System.Action(() => OnButtonClick()));
        }

        private void OnButtonClick()
        {
            SetSettingsData();
        }

        private void SetSettingsData()
        {
            ActorSettingsView settingsView = MenuUI.Instance.ActorSettingsView;
            settingsView.actorData = actorData;
            settingsView.actorPortrait.texture = settingsView.actorData.AvatarPortrait;

            MenuUI.Instance.SetPage("ActorSettings");
        }
    }
}

