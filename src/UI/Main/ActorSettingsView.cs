using NEP.MonoDirector.Core;
using NEP.MonoDirector.Actors;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorSettingsView : MonoBehaviour
    {
        public ActorSettingsView(System.IntPtr ptr) : base(ptr) { }

        private Actor actor;

        private TextMeshProUGUI actorName;

        private Button visibilityButton;
        private Button recastButton;
        private Button deleteButton;

        private bool visible;

        private void Awake()
        {
            actorName = transform.Find("ActorName").GetComponent<TextMeshProUGUI>();
            visibilityButton = transform.Find("OptionsGroup/Visibility").GetComponent<Button>();
            recastButton = transform.Find("OptionsGroup/Recast").GetComponent<Button>();
            deleteButton = transform.Find("OptionsGroup/Delete").GetComponent<Button>();
        }

        public void SetActor(Actor actor)
        {
            this.actor = actor;

            actorName.text = $"Actor Name:\n{actor.ActorName}";
        }

        public void OnVisibilityClicked()
        {
            visible = !visible;
            actor.ClonedAvatar.gameObject.SetActive(!visible);
        }

        public void OnRecastClicked()
        {

        }

        public void OnDeleteClicked()
        {
            Director.instance.RemoveActor(actor);
        }
    }
}