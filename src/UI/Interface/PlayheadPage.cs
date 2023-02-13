using UnityEngine;

namespace NEP.MonoDirector.UI.Interface
{
    public class PlayheadPage
    {
        public PlayheadPage(Transform root)
        {
            this.root = root;
            this.gameObject = root.gameObject;
        }

        public GameObject gameObject;

        private Transform root;
    }
}
