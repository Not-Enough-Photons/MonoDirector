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

        private Button recastButton;
        private Button deleteButton;

        private void Awake()
        {
            actorName = transform.Find("ActorName").GetComponent<TextMeshProUGUI>();
            recastButton = transform.Find("OptionsGroup/Recast").GetComponent<Button>();
            deleteButton = transform.Find("OptionsGroup/Delete").GetComponent<Button>();

            deleteButton.onClick.AddListener(new System.Action(() => DeleteActor(actor)));
        }

        public void SetActor(Actor actor)
        {
            this.actor = actor;

            actorName.text = $"Actor Name:\n{actor.ActorName}";
        }

        public void DeleteActor(Actor actor)
        {
            Director.instance.RemoveActor(actor);
        }
    }
}