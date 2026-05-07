using System.Collections;
using System.Collections.Generic;
using NEP.MonoDirector.UI.Interaction;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI.Menus;

[MelonLoader.RegisterTypeInIl2Cpp]
public class SettingsPage(IntPtr ptr) : MonoBehaviour(ptr)
{
    private Menu m_menu;

    private UIButton m_buttonAudio;
    private UIButton m_buttonCamera;
    private UIButton m_buttonWorld;
    private UIButton m_buttonDebug;
    private UIButton m_buttonCredits;

    private GameObject m_pageAudio;
    private GameObject m_pageCamera;
    private GameObject m_pageCredits;

    private void Awake()
    {
        m_menu = GetComponentInParent<Menu>();
        
        Transform optionsGroup = transform.GetChild(0);

        m_buttonAudio = optionsGroup.Find("Option_Audio").GetComponent<UIButton>();
        m_buttonCamera = optionsGroup.Find("Option_Camera").GetComponent<UIButton>();
        m_buttonWorld = optionsGroup.Find("Option_World").GetComponent<UIButton>();
        m_buttonDebug = optionsGroup.Find("Option_Debug").GetComponent<UIButton>();
        m_buttonCredits = optionsGroup.Find("Option_Credits").GetComponent<UIButton>();
    }

    private void OnEnable()
    {
        m_buttonWorld.OnClicked += OnWorldButtonClicked;
    }

    private void OnDisable()
    {
        m_buttonWorld.OnClicked -= OnWorldButtonClicked;
    }

    private void OnWorldButtonClicked()
    {
        m_menu.GoToPage("World");
    }
}
