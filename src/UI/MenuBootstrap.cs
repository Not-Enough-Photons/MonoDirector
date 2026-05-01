using Il2CppSLZ.Bonelab;
using NEP.MonoDirector.Data;
using UnityEngine;
using UnityEngine.UI;
using Page = BoneLib.BoneMenu.Page;

namespace NEP.MonoDirector.UI
{
    public static class MenuBootstrap
    {
        private static PopUpMenuView m_popUpMenuView;
        private static PreferencesPanelView m_panelView;
        private static GameObject m_gridView;
        private static Button m_button;

        internal static void Initialize(UIRig rig)
        {
            m_popUpMenuView = rig.popUpMenu;
            m_panelView = m_popUpMenuView.preferencesPanelView;
            Transform pageTransform = m_panelView.pages[m_panelView.defaultPage].transform;
            m_gridView = pageTransform.Find("grid_Options").gameObject;
            InjectButton();
        }

        internal static void InjectButton()
        {
            GameObject buttonObject = GameObject.Instantiate(BundleLoader.MenuButtonObject, m_gridView.transform);
            buttonObject.SetActive(true);
            buttonObject.transform.SetSiblingIndex(6);
            m_button = buttonObject.GetComponent<Button>();

            var buttonAction = () =>
            {
                Menu.Instance.Show();
                HideBoneMenu();
            };
            
            m_button.onClick.AddListener(buttonAction);
        }

        public static void OpenPage(Page page)
        {
            if (!m_panelView.isActiveAndEnabled)
                m_panelView.Activate();

            m_popUpMenuView.m_IsActivated = true;
            m_popUpMenuView.m_IsRadialMenu = false;
            
            m_panelView.PAGESELECT(11);
            BoneLib.BoneMenu.Menu.OpenPage(page);
        }

        public static void HideBoneMenu()
        {
            m_panelView.Deactivate();
            //m_popUpMenuView.m_IsRadialMenu = false;
            m_popUpMenuView.m_IsCursorHidden = true;
            m_popUpMenuView.m_IsCursorShown = false;
        }
    }
}
