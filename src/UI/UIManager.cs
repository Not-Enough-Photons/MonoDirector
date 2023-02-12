using NEP.MonoDirector.UI.Debug;
using UnityEngine;

namespace NEP.MonoDirector.UI
{
    public static class UIManager
    {
        public static DeveloperUI developerUI;

        public static void Construct()
        {
            developerUI = new DeveloperUI();
        }

        public static void Update()
        {
            developerUI?.Update();
        }
    }
}
