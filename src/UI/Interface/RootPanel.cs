using BoneLib;
using SLZ.Bonelab;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MelonLoader.MelonLogger;

namespace NEP.MonoDirector.UI.Interface
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class RootPanel : MonoBehaviour
    {
        public RootPanel(System.IntPtr ptr) : base(ptr) { }

        public TextMeshProUGUI text_Title;

        public ActorPage actorPage;
        public PlayheadPage playheadPage;

        public GameObject activePage;

        public InteractableInputModule eventSystemModule;

        private Transform header;
        private Transform pages;

        private Button btn_Close;

        private GameObject[] pageList;

        private void Awake()
        {
            header = transform.Find("Header");
            pages = transform.Find("Pages");

            btn_Close = header.Find("button_Close").GetComponent<Button>();
            text_Title = header.Find("text_Title").GetComponent<TextMeshProUGUI>();

            pageList = new GameObject[pages.childCount];

            for(int i = 0; i < pages.childCount; i++)
            {
                pageList[i] = pages.GetChild(i).gameObject;
            }

            actorPage = new ActorPage(pageList[0].transform);
            playheadPage = new PlayheadPage(pageList[1].transform);

            btn_Close.onClick.AddListener(new System.Action(() => ClosePage()));

            eventSystemModule = GameObject.FindObjectOfType<InteractableInputModule>();
        }

        private void FixedUpdate()
        {
            Quaternion lookRot = Quaternion.LookRotation(Player.physicsRig.m_head.forward);

            transform.rotation = lookRot;
        }

        public void SetActivePage(GameObject page)
        {
            for(int i = 0; i < pageList.Length; i++)
            {
                if (pageList[i] == page)
                {
                    pageList[i].SetActive(true);
                }
                else
                {
                    pageList[i].SetActive(false);
                }
            }
        }

        public void ClosePage()
        {
            gameObject.SetActive(false);
        }
    }
}
