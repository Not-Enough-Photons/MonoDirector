using NEP.MonoDirector.Core;
using System;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class GripEventListener : MonoBehaviour
    {
        public GripEventListener(IntPtr ptr) : base(ptr) { }

        private SimpleGripEvents gripEvents;

        private Prop prop;

        private Action onAttach;
        private Action onDetach;
        private Action onIndexDown;
        private Action onMenuTapDown;

        private void Awake()
        {
            gripEvents = GetComponent<SimpleGripEvents>();

            onAttach = new Action(OnAttach);
            onDetach = new Action(OnDetach);
            onIndexDown = new Action(OnIndexDown);
            onMenuTapDown = new Action(OnMenuTapDown);

            gripEvents.OnAttach.AddListener(onAttach);
            gripEvents.OnDetach.AddListener(onDetach);
            gripEvents.OnIndexDown.AddListener(onIndexDown);
            gripEvents.OnMenuTapDown.AddListener(onMenuTapDown);
        }

        private void Start()
        {
            Events.OnStopRecording += new Action(() =>
            {
                gripEvents.OnAttach.RemoveListener(onAttach);
                gripEvents.OnDetach.RemoveListener(onDetach);
                gripEvents.OnIndexDown.RemoveListener(onIndexDown);
                gripEvents.OnMenuTapDown.RemoveListener(onMenuTapDown);
            });
        }

        private void OnDestroy()
        {

        }

        public void SetProp(Prop prop)
        {
            this.prop = prop;
        }

        private void OnAttach()
        {
            if(Director.PlayState != State.PlayState.Recording)
            {
                return;
            }

            Main.Logger.Msg("OnAttach");

            if(prop == null)
            {
                return;
            }

            prop.RecordAction(new System.Action(() => gripEvents.OnAttach?.Invoke()));
        }

        private void OnDetach()
        {
            if (Director.PlayState != State.PlayState.Recording)
            {
                return;
            }

            Main.Logger.Msg("OnDetach");

            if (prop == null)
            {
                return;
            }

            prop.RecordAction(new System.Action(() => gripEvents.OnDetach?.Invoke()));
        }

        private void OnIndexDown()
        {
            if (Director.PlayState != State.PlayState.Recording)
            {
                return;
            }

            Main.Logger.Msg("OnIndexDown");

            if (prop == null)
            {
                return;
            }

            prop.RecordAction(new System.Action(() => gripEvents.OnIndexDown?.Invoke()));
        }

        private void OnMenuTapDown()
        {
            if (Director.PlayState != State.PlayState.Recording)
            {
                return;
            }

            Main.Logger.Msg("OnMenuTapDown");

            if (prop == null)
            {
                return;
            }

            prop.RecordAction(new System.Action(() => gripEvents.OnMenuTapDown?.Invoke()));
        }
    }
}
