using Il2CppSLZ.Bonelab;

using MelonLoader;
using NEP.MonoDirector.Core;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI.Interaction;

[RegisterTypeInIl2Cpp]
public class UIButton(IntPtr ptr) : MonoBehaviour(ptr)
{
    public event Action OnClicked;
    
    private const float MaxTolerance = 0.01f;
    private const float ValueEpsilon = 0.001f;

    private Rigidbody m_body;
    private Button m_button;

    private Feedback_Audio m_feedbackAudio;
    
    private float m_value;
    
    private bool m_pressed;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_body = GetComponent<Rigidbody>();

        m_feedbackAudio = GetComponent<Feedback_Audio>();
    }

    private void OnEnable()
    {
        if (!m_body)
            return;

        m_body.velocity = Vector3.zero;
        m_body.angularVelocity = Vector3.zero;

        m_value = 0f;
    }

    private void Update()
    {
        if (!m_button)
            return;

        if (!m_button.targetGraphic)
            return;

        m_button.targetGraphic.color =
            Color.Lerp(m_button.colors.normalColor, m_button.colors.pressedColor, m_value);
    }

    private void FixedUpdate()
    {
        float curDistance = Mathf.Abs(transform.localPosition.z);
        float maxDistance = MaxTolerance - ValueEpsilon;

        m_value = Mathf.Clamp01(curDistance / maxDistance);
        
        if (curDistance >= maxDistance)
        {
            if (!m_pressed)
                Press();
        }
        else
        {
            if (m_pressed)
                Release();
        }
    }

    private void Press()
    {
        m_pressed = true;

        if (m_feedbackAudio.clips_Click == null)
            return;

        BoneLib.Audio.Play2DOneShot(m_feedbackAudio.clips_Click, BoneLib.Audio.UI);
    }

    private void Release()
    {
        m_pressed = false;
        OnClicked?.Invoke();

        if (m_feedbackAudio.clips_Deny == null)
            return;

        BoneLib.Audio.Play2DOneShot(m_feedbackAudio.clips_Deny, BoneLib.Audio.UI);
    }
}