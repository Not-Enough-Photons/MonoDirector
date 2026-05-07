using MelonLoader;
using NEP.MonoDirector.UI.Interaction;
using UnityEngine;

namespace NEP.MonoDirector.UI.Menus;

[RegisterTypeInIl2Cpp]
public class AudioPage(IntPtr ptr) : MonoBehaviour(ptr)
{
    private UIButton m_buttonEnableMicRecord;
    private UIButton m_buttonEnableMicPlayback;

    private GameObject m_micRecordCheck;
    private GameObject m_micPlaybackCheck;

    private void Awake()
    {
        m_buttonEnableMicRecord = transform.Find("Button_EnableMicRecord").GetComponent<UIButton>();
        m_buttonEnableMicPlayback = transform.Find("Button_EnableMicPlayback").GetComponent<UIButton>();

        m_micRecordCheck = m_buttonEnableMicRecord.transform.Find("Checkbox/Check").gameObject;
        m_micPlaybackCheck = m_buttonEnableMicPlayback.transform.Find("Checkbox/Check").gameObject;
    }

    private void OnEnable()
    {
        m_buttonEnableMicRecord.OnClicked += OnEnableMicRecordClicked;
        m_buttonEnableMicPlayback.OnClicked += OnEnableMicPlaybackClicked;
        
        m_micRecordCheck.SetActive(Settings.World.useMicrophone);
        m_micPlaybackCheck.SetActive(Settings.World.micPlayback);
    }

    private void OnDisable()
    {
        m_buttonEnableMicRecord.OnClicked -= OnEnableMicRecordClicked;
        m_buttonEnableMicPlayback.OnClicked -= OnEnableMicPlaybackClicked;
    }

    private void OnEnableMicRecordClicked()
    {
        Settings.World.useMicrophone = !Settings.World.useMicrophone;
        m_micRecordCheck.SetActive(Settings.World.useMicrophone);
    }
    
    private void OnEnableMicPlaybackClicked()
    {
        Settings.World.micPlayback = !Settings.World.micPlayback;
        m_micPlaybackCheck.SetActive(Settings.World.micPlayback);
    }
}