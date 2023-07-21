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
        public TextMeshProUGUI avatarNameText;
        private Button button;

        public Actor actorData;

        public void Start()
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
            actorData = actor;
            avatarNameText.text = actor.ActorName;
        }

        private void OnButtonClick()
        {
            MenuUI.Instance.ActorSettingsView.SetActor(actorData);
            MenuUI.Instance.SetPage("ActorSettings");
        }
    }
}

