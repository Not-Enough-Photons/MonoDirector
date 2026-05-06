using NEP.MonoDirector.UI.Interaction;

using BoneLib;
using Il2CppTMPro;
using MelonLoader;
using NEP.MonoDirector.Tools;
using UnityEngine;

namespace NEP.MonoDirector.UI.Menus;

[RegisterTypeInIl2Cpp]
public class Menu(IntPtr ptr) : MonoBehaviour(ptr)
{
    public static Menu Instance { get; private set; }

    public GameObject DefaultPage => m_defaultPage;
    
    private List<GameObject> m_pages;
    private GameObject m_defaultPage;
    private GameObject m_currentPage;
    private GameObject m_previousPage;

    private UIButton m_closeButton;
    private UIButton m_backButton;

    private TextMeshProUGUI m_title;
    
    private void Awake()
    {
        Instance = this;

        m_pages = new List<GameObject>();

        Transform pageGroup = transform.Find("Body/Pages");

        for (int i = 0; i < pageGroup.childCount; i++)
        {
            GameObject pageObject = pageGroup.GetChild(i).gameObject;
            pageObject.SetActive(false);
            m_pages.Add(pageObject);
        }
        
        m_closeButton = transform.Find("Header/Button").GetComponent<UIButton>();
        m_backButton = transform.Find("Header/Back").GetComponent<UIButton>();
        m_title = transform.Find("Header/Title").GetComponent<TextMeshProUGUI>();
        
        m_defaultPage = GetPage("Menu");
        GoToPage(m_defaultPage.name);
    }

    private void OnEnable()
    {
        m_closeButton.OnClicked += OnCloseButtonClicked;
        m_backButton.OnClicked += OnBackButtonClicked;
    }

    private void OnDisable()
    {
        m_closeButton.OnClicked -= OnCloseButtonClicked;
        m_backButton.OnClicked -= OnBackButtonClicked;
    }

    public void Teleport()
    {
        Transform playerChest = BoneLib.Player.PhysicsRig.m_chest;
        transform.position = playerChest.position + playerChest.forward;
        // Calculate look at
        Vector3 lookRotation = Quaternion.LookRotation(playerChest.position - transform.position).eulerAngles;
        Quaternion yRotation = Quaternion.Euler(0f, lookRotation.y + 180f, 0f);
        transform.rotation = yRotation;
    }
    
    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);

    public GameObject GetPage(string name)
    {
        foreach (var page in m_pages)
        {
            if (page.name != name)
                continue;

            return page;
        }

        return null;
    }

    public void GoToPage(string name)
    {
        foreach (var pageToDisable in m_pages)
            pageToDisable.SetActive(false);
        
        m_previousPage = m_currentPage;
        GameObject page = GetPage(name);
        m_currentPage = page;
        m_currentPage.SetActive(true);
        m_title.text = m_currentPage.name;
        
        if (m_currentPage == m_defaultPage)
            m_backButton.gameObject.SetActive(false);
        else
            m_backButton.gameObject.SetActive(true);
    }

    private void OnBackButtonClicked()
    {
        string pageName = m_previousPage.name;
        GoToPage(pageName);
    }
    
    private void OnCloseButtonClicked()
    {
        Hide();
    }
}
