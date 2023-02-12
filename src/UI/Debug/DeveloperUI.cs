using NEP.MonoDirector.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI.Debug
{
    public class DeveloperUI
    {
        public DeveloperUI()
        {
            var obj = Main.bundle.LoadAsset("UI_Canvas_Debug").Cast<GameObject>();
            canvasObject = GameObject.Instantiate(obj);
            canvasObject.hideFlags = HideFlags.DontUnloadUnusedAsset;

            txt_actorCount = canvasObject.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            txt_currentTick = canvasObject.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            txt_recordedTicks = canvasObject.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
            txt_playbackTicks = canvasObject.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>();

            btn_Record = canvasObject.transform.GetChild(1).GetChild(0).GetComponent<Button>();
            btn_Play = canvasObject.transform.GetChild(1).GetChild(1).GetComponent<Button>();
            btn_Stop = canvasObject.transform.GetChild(1).GetChild(2).GetComponent<Button>();

            btn_Record.onClick.AddListener(new System.Action(() => Director.instance.Record()));
            btn_Play.onClick.AddListener(new System.Action(() => Director.instance.Play()));
            btn_Stop.onClick.AddListener(new System.Action(() => Director.instance.Stop()));
        }

        public GameObject debugCanvas;

        // [Section] - Counters
        public TextMeshProUGUI txt_actorCount;
        public TextMeshProUGUI txt_currentTick;
        public TextMeshProUGUI txt_recordedTicks;
        public TextMeshProUGUI txt_playbackTicks;

        // [Section] - Buttons
        public Button btn_Record;
        public Button btn_Play;
        public Button btn_Stop;

        private GameObject canvasObject;

        public void Update()
        {
            try
            {

            }
            catch
            {
                txt_actorCount.text = "Actor Count: " + Director.instance?.Cast.Count.ToString();
                txt_currentTick.text = "Current Tick: " + Playback.instance?.PlaybackTick.ToString();
                txt_recordedTicks.text = "Recording Tick: " + Recorder.instance?.RecordTick.ToString();
            }
        }
    }
}
