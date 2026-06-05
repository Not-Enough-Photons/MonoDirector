using MelonLoader;
using NEP.MonoDirector.Core;
using UnityEngine;

using NEP.MonoDirector.UI.Interaction;

namespace NEP.MonoDirector.UI.Menus;

[RegisterTypeInIl2Cpp]
public class PlaybackPage(IntPtr ptr) : MonoBehaviour(ptr)
{
    private UIButton m_recordButton;
    private UIButton m_playButton;
    private UIButton m_stopButton;

    private UIButton m_saveButton;
    private UIButton m_loadButton;
    
    private void Awake()
    {
        Transform optionGroup = transform.Find("Options");
        Transform saverOptionGroup = transform.Find("SaverOptions");

        m_recordButton = optionGroup.Find("Record").GetComponent<UIButton>();
        m_playButton = optionGroup.Find("Play").GetComponent<UIButton>();
        m_stopButton = optionGroup.Find("Stop").GetComponent<UIButton>();

        m_saveButton = saverOptionGroup.transform.Find("Save").GetComponent<UIButton>();
        m_loadButton = saverOptionGroup.transform.Find("Load").GetComponent<UIButton>();
    }

    private void OnEnable()
    {
        m_recordButton.OnClicked += OnRecordPressed;
        m_playButton.OnClicked += OnPlayPressed;
        m_stopButton.OnClicked += OnStopPressed;

        m_saveButton.OnClicked += OnSaveClicked;
        m_loadButton.OnClicked += OnLoadClicked;
    }

    private void OnDisable()
    {
        m_recordButton.OnClicked -= OnRecordPressed;
        m_playButton.OnClicked -= OnPlayPressed;
        m_stopButton.OnClicked -= OnStopPressed;
        
        m_saveButton.OnClicked -= OnSaveClicked;
        m_loadButton.OnClicked -= OnLoadClicked;
    }

    private void OnRecordPressed()
    {
        Menu.Instance.Hide();
        Director.Record();
    }

    private void OnPlayPressed()
    {
        Menu.Instance.Hide();
        Director.Play();
    }

    private void OnStopPressed()
    {
        Director.Stop();
    }

    private void OnSaveClicked()
    {
        Director.Save("test");
    }

    private void OnLoadClicked()
    {
        Director.Load("test");
    }
}