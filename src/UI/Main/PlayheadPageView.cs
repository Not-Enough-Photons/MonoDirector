using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class PlayheadPageView : MonoBehaviour
    {
        public PlayheadPageView(System.IntPtr ptr) : base(ptr) { }

        private Button button_Play;
        private Button button_Record;
        private Button button_Stop;

        private TextMeshProUGUI timecodeText;

        private void Awake()
        {
            button_Record = transform.Find("Record").GetComponent<Button>();
            button_Play = transform.Find("Play").GetComponent<Button>();
            button_Stop = transform.Find("Stop").GetComponent<Button>();
            timecodeText = transform.Find("Duration").GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            button_Record.onClick.AddListener(new System.Action(() => Director.instance.Record()));
            button_Play.onClick.AddListener(new System.Action(() => Director.instance.Play()));
            button_Stop.onClick.AddListener(new System.Action(() => Director.instance.Stop()));
        }

        private void Update()
        {
            timecodeText.text = Recorder.instance.TakeTime.ToString();
        }
    }
}
