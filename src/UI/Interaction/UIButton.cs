using Il2CppSLZ.Bonelab;

using MelonLoader;

using UnityEngine;
using UnityEngine.UI;


namespace NEP.MonoDirector.UI.Interaction
{
    [RegisterTypeInIl2Cpp]
    public class UIButton(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public event Action OnClicked;
        
        private const float MaxTolerance = 0.01f;
        private const float ValueEpsilon = 0.001f;

        private Transform m_contactTransform;
        private Rigidbody m_body;
        private Button m_button;

        private Feedback_Audio m_feedbackAudio;
        
        private float m_value;
        
        private bool m_pressed;

        private void Awake()
        {
            m_button = GetComponent<Button>();
            m_contactTransform = transform.Find("Contact");
            m_body = m_contactTransform.GetComponent<Rigidbody>();

            m_feedbackAudio = GetComponent<Feedback_Audio>();
        }

        private void OnEnable()
        {
            m_body.velocity = Vector3.zero;
            m_body.angularVelocity = Vector3.zero;
            m_contactTransform.localPosition = Vector3.zero;
            m_contactTransform.localRotation = Quaternion.identity;

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
            float curDistance = m_contactTransform.localPosition.z;
            float maxDistance = MaxTolerance - ValueEpsilon;

            m_value = Mathf.Clamp01(Mathf.Abs(curDistance / maxDistance));
            
            if (curDistance <= -maxDistance)
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
            BoneLib.Audio.Play2DOneShot(m_feedbackAudio.clips_Click, BoneLib.Audio.UI);
        }

        private void Release()
        {
            m_pressed = false;
            OnClicked?.Invoke();
            
            BoneLib.Audio.Play2DOneShot(m_feedbackAudio.clips_Deny, BoneLib.Audio.UI);
        }
    }
}