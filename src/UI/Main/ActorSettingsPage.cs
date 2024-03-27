using TMPro;
using UnityEngine;
using UnityEngine.UI;

using NEP.MonoDirector.Actors;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorSettingsPage : MonoBehaviour
    {
        public ActorSettingsPage(System.IntPtr ptr) : base(ptr) { }

        private MDMenu menu;

        private RawImage actorPortrait;
        private TextMeshProUGUI actorNameText;

        private Button visibilityButton;
        private Button recastButton;
        private Button deleteButton;

        private Actor actor;

        private bool initialized = false;

        public void Initialize(MDMenu menu)
        {
            if (initialized)
            {
                return;
            }

            this.menu = menu;

            actorPortrait = transform.GetChild(0).GetComponent<RawImage>();
            actorNameText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            Transform optionsGroup = transform.Find("OptionsGroup");
            visibilityButton = optionsGroup.GetChild(0).GetComponent<Button>();
            recastButton = optionsGroup.GetChild(1).GetComponent<Button>();
            deleteButton = optionsGroup.GetChild(2).GetComponent<Button>();
        }

        public void UpdateInformation(Actor actor)
        {
            this.actor = actor;
            // actorPortrait.texture = this.actor.actorPortrait;
            actorNameText.text = this.actor.ActorName;
        }
    }
}