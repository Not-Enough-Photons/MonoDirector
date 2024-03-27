using TMPro;
using UnityEngine.UI;
using UnityEngine;

using NEP.MonoDirector.Actors;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorEntry : MonoBehaviour
    {
        public ActorEntry(System.IntPtr ptr) : base(ptr) { }

        public RawImage avatarImage;
        public TextMeshProUGUI avatarNameText;
        public Button avatarButton;

        private Actor actor;

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        public Actor GetActor()
        {
            return this.actor;
        }

        public void SetActor(Actor actor)
        {
            this.actor = actor;
        }
    }
}