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

        public Actor actorData;
        public RawImage actorPortrait;

        private TextMeshProUGUI actorName;

        private Button visibilityButton;
        private Button recastButton;
        private Button deleteButton;

        private bool visible;

        private void Awake()
        {
            actorName = transform.Find("ActorName").GetComponent<TextMeshProUGUI>();
            actorPortrait = transform.Find("Avatar").GetComponent<RawImage>();
            visibilityButton = transform.Find("OptionsGroup/Visibility").GetComponent<Button>();
            recastButton = transform.Find("OptionsGroup/Recast").GetComponent<Button>();
            deleteButton = transform.Find("OptionsGroup/Delete").GetComponent<Button>();
        }

        private void Start()
        {
            visibilityButton.onClick.AddListener(new System.Action(() => OnVisibilityClicked()));
        }

        private void OnEnable()
        {
            actorName.text = $"Actor Name:\n{actorData.ActorName}";
        }

        public void OnVisibilityClicked()
        {
            visible = !visible;
            actorData.ClonedAvatar.gameObject.SetActive(!visible);
        }

        public void OnRecastClicked()
        {

        }

        public void OnDeleteClicked()
        {
            Director.instance.RemoveActor(actorData);
        }
    }
}