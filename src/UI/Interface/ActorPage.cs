using TMPro;

using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System.Collections.Generic;
using NEP.MonoDirector.Actors;

namespace NEP.MonoDirector.UI.Interface
{
    public class ActorPage
    {
        public struct ActorEntry
        {
            public ActorEntry(Transform groupRoot)
            {
                this.avatar = groupRoot.GetChild(0).GetComponent<Image>();
                this.textComponent = groupRoot.GetChild(1).GetComponent<TextMeshProUGUI>();
                this.avatarItem = groupRoot.gameObject;
            }

            public Image avatar;
            public TextMeshProUGUI textComponent;
            public GameObject avatarItem;
        }

        public struct ActorGroup
        {
            public ActorGroup(Transform groupRoot)
            {
                entries = new ActorEntry[groupRoot.childCount];

                for(int i = 0; i < groupRoot.childCount; i++)
                {
                    var current = groupRoot.GetChild(i);
                    var entry = new ActorEntry(current);

                    this.entries[i] = entry;
                }
            }

            public ActorEntry[] entries;
        }

        public struct SettingsGroup
        {
            public SettingsGroup(Transform groupRoot)
            {
                removeButton = groupRoot.GetChild(0).GetComponent<Button>();
            }

            public Button removeButton;
        }

        public struct PageControlGroup
        {
            public Button previousPageButton;
            public Button nextPageButton;

            private Transform groupRoot;
        }

        public ActorPage(Transform root)
        {
            this.root = root;
            this.gameObject = root.gameObject;
            Initialize();
        }

        public ActorGroup actorGroup;
        public SettingsGroup settingsGroup;
        public PageControlGroup pageControlGroup;

        public GameObject gameObject;

        private Transform root;

        private void Initialize()
        {
            actorGroup = new ActorGroup(root.Find("group_CastList"));
            settingsGroup = new SettingsGroup(root.Find("group_Settings"));

            foreach(var entry in actorGroup.entries)
            {
                entry.avatarItem.SetActive(false);
            }

            Events.OnActorCasted += AppendActor;
        }

        public void AppendActor(Actor actor)
        {
            var test = actorGroup.entries.First((item) => !item.avatarItem.activeInHierarchy);
            test.avatarItem.SetActive(true);
            test.textComponent.text = actor.ActorName;
        }
    }
}
