using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class MDFooter : MonoBehaviour
    {
        public MDFooter(System.IntPtr ptr) : base(ptr) { }

        private Button previousPageButton;
        private Button nextPageButton;
        private Button goBackButton;
        private TextMeshProUGUI pageText;

        private ActorsPage actorsPage;

        private void Awake()
        {
            previousPageButton = transform.Find("PreviousPage").GetComponent<Button>();
            nextPageButton = transform.Find("NextPage").GetComponent<Button>();
            goBackButton = transform.Find("GoBack").GetComponent<Button>();
            pageText = transform.Find("PageText").GetComponent<TextMeshProUGUI>();
        }

        public void LinkToActorView(ActorsPage actorsPage)
        {
            this.actorsPage = actorsPage;

            Action previousPageClicked = new Action(() => 
            {
                actorsPage.PreviousPage();
                pageText.text = $"Page {actorsPage.PageIndex}/{actorsPage.PageCount}";
            });

            Action nextPageClicked = new Action(() => 
            {
                actorsPage.NextPage();
                pageText.text = $"Page {actorsPage.PageIndex}/{actorsPage.PageCount}";
            });

            Action goBackClicked = new Action(() => 
            {
                actorsPage.GoBack();
            });

            previousPageButton.onClick.AddListener(previousPageClicked);
            nextPageButton.onClick.AddListener(nextPageClicked);
            goBackButton.onClick.AddListener(goBackClicked);
        }

        public void UnlinkToActorView()
        {
            
        }
    }
}