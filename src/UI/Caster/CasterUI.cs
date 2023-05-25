using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using System;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class CasterUI : MonoBehaviour
    {
        public CasterUI(System.IntPtr ptr) : base(ptr) { }

        public static CasterUI Instance { get; private set; }

        public CasterPageView casterPageView;
        public ActorSettingsView actorSettingsView;

        private GameObject castPage;
        private GameObject actorSettingsPage;

        private Transform footer;
        private Transform pageList;

        private TextMeshProUGUI pageText;

        private Button previousPageButton;
        private Button nextPageButton;
        private Button goBackButton;

        private void Awake()
        {
            Instance = this;

            footer = transform.Find("Footer");
            pageList = transform.Find("Content/Pages");

            castPage = pageList.Find("Cast").gameObject;
            actorSettingsPage = pageList.Find("ActorSettings").gameObject;

            pageText = footer.Find("PageText").GetComponent<TextMeshProUGUI>();

            previousPageButton = footer.Find("PreviousPage").GetComponent<Button>();
            nextPageButton = footer.Find("NextPage").GetComponent<Button>();
            goBackButton = footer.Find("BackButton").GetComponent<Button>();

            casterPageView = castPage.GetComponent<CasterPageView>();
            actorSettingsView = actorSettingsPage.GetComponent<ActorSettingsView>();

            previousPageButton.onClick.AddListener(new System.Action(() => casterPageView.PreviousPage()));
            nextPageButton.onClick.AddListener(new System.Action(() => casterPageView.NextPage()));
            goBackButton.onClick.AddListener(new System.Action(() => SetPage("Cast")));

            actorSettingsView.gameObject.SetActive(false);
        }

        private void Update()
        {
            var physicsRig = Constants.rigManager.physicsRig;

            transform.position = Vector3.Lerp(transform.position, physicsRig.m_chest.position + physicsRig.m_chest.forward * 1.5f, 16f * Time.deltaTime);
            transform.LookAt(physicsRig.m_chest);
        }

        public void UpdatePageCounter(int currentPage, int maxPages)
        {
            pageText.text = $"Page {currentPage} / {maxPages}";
        }

        public void SetPage(string page)
        {
            if(page == "Cast")
            {
                castPage.SetActive(true);
                actorSettingsPage.SetActive(false);
                pageText.gameObject.SetActive(true);
                goBackButton.gameObject.SetActive(false);

                previousPageButton.gameObject.SetActive(true);
                nextPageButton.gameObject.SetActive(true);
            }

            if(page == "ActorSettings")
            {
                castPage.SetActive(false);
                actorSettingsPage.SetActive(true);
                pageText.gameObject.SetActive(false);
                goBackButton.gameObject.SetActive(true);

                previousPageButton.gameObject.SetActive(false);
                nextPageButton.gameObject.SetActive(false);
            }
        }
    }
}