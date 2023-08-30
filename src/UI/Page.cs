
using UnityEngine;

namespace NEP.MonoDirector.UI
{
    public class Page
    {
        public Page(string name, Transform pageTransform)
        {
            Name = name;
            this.pageTransform = pageTransform;
        }

        public string Name { get; private set; }

        private Transform pageTransform;
    }
}
