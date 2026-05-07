using MelonLoader;
using NEP.MonoDirector.UI.Interaction;
using UnityEngine;

namespace NEP.MonoDirector.UI.Menus;

[RegisterTypeInIl2Cpp]
public class WorldPage(IntPtr ptr) : MonoBehaviour(ptr)
{
    private UIButton m_buttonRecordActors;
    private UIButton m_buttonShowGizmos;

    private GameObject m_recordActorsCheck;
    private GameObject m_showGizmosCheck;

    private void Awake()
    {
        m_buttonRecordActors = transform.Find("Button_RecordActors").GetComponent<UIButton>();
        m_buttonShowGizmos = transform.Find("Button_ShowGizmos").GetComponent<UIButton>();

        m_recordActorsCheck = m_buttonRecordActors.transform.Find("Checkbox/Check").gameObject;
        m_showGizmosCheck = m_buttonShowGizmos.transform.Find("Checkbox/Check").gameObject;
    }

    private void OnEnable()
    {
        m_buttonRecordActors.OnClicked += OnRecordActorsClicked;
        m_buttonShowGizmos.OnClicked += OnShowGizmosClicked;
        
        m_recordActorsCheck.SetActive(Settings.World.recordActors);
        m_showGizmosCheck.SetActive(Settings.World.showGizmos);
    }

    private void OnDisable()
    {
        m_buttonRecordActors.OnClicked -= OnRecordActorsClicked;
        m_buttonShowGizmos.OnClicked -= OnShowGizmosClicked;
    }

    private void OnRecordActorsClicked()
    {
        Settings.World.recordActors = !Settings.World.recordActors;
        m_recordActorsCheck.SetActive(Settings.World.recordActors);
    }
    
    private void OnShowGizmosClicked()
    {
        Settings.World.showGizmos = !Settings.World.showGizmos;
        m_showGizmosCheck.SetActive(Settings.World.showGizmos);
    }
}