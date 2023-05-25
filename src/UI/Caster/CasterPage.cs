using NEP.MonoDirector.Core;
using NEP.MonoDirector.Actors;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace NEP.MonoDirector.UI
{
    [System.Serializable]
    public class CasterPage
    {
        public CasterPage(CasterPageView pageView)
        {
            actors = new List<Actor>();

            this.pageView = pageView;

            Events.OnActorCasted += OnActorCasted;
        }

        public List<Actor> actors;

        private int pageElements = 0;
        private int maxElementsPerPage = 8;

        private bool filled = false;

        private CasterPageView pageView;

        public void OnActorCasted(Trackable trackable)
        {
            Actor actor = (Actor)trackable;
            AddActor(this, actor);
        }

        public void AddActor(CasterPage page, Actor actor)
        {
            if (filled)
            {
                return;
            }

            if (pageElements + 1 > maxElementsPerPage)
            {
                if (!filled)
                {
                    filled = true;
                }

                CasterPage newPage = new CasterPage(pageView);
                pageView.AddPage(newPage);
                newPage.AddActor(newPage, actor);
                return;
            }
            else
            {
                pageElements++;
            }

            actors.Add(actor);

            if (this != pageView.ActivePage)
            {
                return;
            }

            var entry = pageView.EntryObjects.FirstOrDefault((entryObject) => !entryObject.gameObject.activeInHierarchy);
            entry.SetActor(actor);
            entry.gameObject.SetActive(true);
        }
    }
}
