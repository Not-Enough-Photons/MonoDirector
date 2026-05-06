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

    private void Awake()
    {
        Transform optionGroup = transform.Find("Options");

        m_recordButton = optionGroup.Find("Record").GetComponent<UIButton>();
        m_playButton = optionGroup.Find("Play").GetComponent<UIButton>();
        m_stopButton = optionGroup.Find("Stop").GetComponent<UIButton>();
    }

    private void OnEnable()
    {
        m_recordButton.OnClicked += OnRecordPressed;
        m_playButton.OnClicked += OnPlayPressed;
        m_stopButton.OnClicked += OnStopPressed;
    }

    private void OnDisable()
    {
        m_recordButton.OnClicked -= OnRecordPressed;
        m_playButton.OnClicked -= OnPlayPressed;
        m_stopButton.OnClicked -= OnStopPressed;
    }

    private void OnRecordPressed()
    {
        Director.Record();
    }

    private void OnPlayPressed()
    {
        Director.Play();
    }

    private void OnStopPressed()
    {
        Director.Stop();
    }
}