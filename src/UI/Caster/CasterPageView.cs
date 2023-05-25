using System.Collections.Generic;

using UnityEngine;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class CasterPageView : MonoBehaviour
    {
        public CasterPageView(System.IntPtr ptr) : base(ptr) { }

        public ActorEntry[] EntryObjects => entryObjects;

        public CasterPage ActivePage => activePage;

        private CasterUI casterUI => CasterUI.Instance;

        private List<CasterPage> pages;

        private CasterPage activePage;

        private ActorEntry[] entryObjects;

        private int currentPage = 0;
        private int totalPages = 0;

        private void Awake()
        {
            pages = new List<CasterPage>();
            entryObjects = transform.Find("CastList").GetComponentsInChildren<ActorEntry>();

            foreach(var entryObject in entryObjects)
            {
                entryObject.Awake();
                entryObject.gameObject.SetActive(false);
            }

            AddPage(new CasterPage(this));
        }

        private void OnEnable()
        {
            SetPage(pages[0]);
        }

        public void AddPage(CasterPage page)
        {
            pages.Add(page);
            totalPages = pages.Count;
            casterUI.UpdatePageCounter(currentPage, totalPages);
        }

        public void SetPage(CasterPage page)
        {
            for(int i = 0; i < pages.Count; i++)
            {
                if (pages[i] == page)
                {
                    activePage = pages[i];
                    currentPage = i;
                    casterUI.UpdatePageCounter(currentPage + 1, totalPages);
                    break;
                }
            }

            DrawElements();
        }

        public void NextPage()
        {
            int nextPageIndex = currentPage + 1;

            if(nextPageIndex >= pages.Count)
            {
                nextPageIndex = pages.Count - 1;
            }

            SetPage(pages[nextPageIndex]);
        }

        public void PreviousPage()
        {
            int previousPageIndex = currentPage - 1;

            if (previousPageIndex <= 0)
            {
                previousPageIndex = 0;
            }

            SetPage(pages[previousPageIndex]);
        }

        private void DrawElements()
        {
            for(int i = 0; i < entryObjects.Length; i++)
            {
                entryObjects[i].gameObject.SetActive(false);

                for (int j = 0; j < activePage.actors.Count; j++)
                {
                    if(activePage.actors.Count == 0)
                    {
                        continue;
                    }
                    else
                    {
                        entryObjects[j].gameObject.SetActive(true);
                        entryObjects[j].SetActor(activePage.actors[j]);
                    }
                }
            }
        }
    }
}

