using Il2CppSLZ.Marrow.Interaction;
using MelonLoader;
using NEP.MonoDirector.Core;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI.Interaction
{
    [RegisterTypeInIl2Cpp]
    public class UIButton(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public Button.ButtonClickedEvent OnClicked => m_button.onClick;
        
        private const float MaxTolerance = 0.01f;
        private const float ValueEpsilon = 0.001f;

        private Transform m_contactTransform;
        private Rigidbody m_body;
        private Button m_button;

        private float m_value;
        
        private bool m_pressed;

        private void Awake()
        {
            m_button = GetComponent<Button>();
            m_contactTransform = transform.Find("Contact");
            m_body = m_contactTransform.GetComponent<Rigidbody>();
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
        }

        private void Release()
        {
            m_pressed = false;
            OnClicked?.Invoke();
        }
    }
}