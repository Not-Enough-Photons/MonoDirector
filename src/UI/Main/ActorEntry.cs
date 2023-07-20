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

        private Image avatarImage;
        private TextMeshProUGUI avatarNameText;
        private Button button;

        public Actor actor;

        public void Awake()
        {
            avatarImage = transform.Find("Avatar").GetComponent<Image>();
            avatarNameText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            button = GetComponent<Button>();

            button.targetGraphic = avatarImage;
            button.onClick.AddListener(new System.Action(() => OnButtonClick()));
        }

        public void SetActor(Actor actor)
        {
            Main.Logger.Msg($"ActorEntry.SetActor({actor.ActorName})");
            this.actor = actor;
            avatarNameText.text = actor.ActorName;
        }

        private void OnButtonClick()
        {
            MenuUI.Instance.SetPage("ActorSettings");
            MenuUI.Instance.ActorSettingsView.SetActor(actor);
        }
    }
}

