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

        private Actor actor;

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
            if(actor == null)
            {
                return;
            }

            this.actor = actor;
            avatarNameText.text = actor.ActorName;
        }

        private void OnButtonClick()
        {
            CasterUI.Instance.SetPage("ActorSettings");
            CasterUI.Instance.actorSettingsView.SetActor(actor);
        }
    }
}

