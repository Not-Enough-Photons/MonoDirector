using NEP.MonoDirector.Actors;
using System;

namespace NEP.MonoDirector
{
    public static class Events
    {
        public static void FlushActions()
        {
            OnActorCasted = null;

            OnSessionBegin = null;
            OnSessionEnd = null;

            OnPrePlayback = null;
            OnPreRecord = null;

            OnPlay = null;
            OnPause = null;
            OnStopPlayback = null;

            OnStartRecording = null;
            OnStopRecording = null;

            OnPlaybackTick = null;
            OnRecordTick = null;

            OnPreSnapshot = null;
        }

        public static Action<Trackable> OnActorCasted;

        public static Action<Prop> OnPropCreated;
        public static Action<Prop> OnPropRemoved;

        public static Action OnSessionBegin;
        public static Action OnSessionEnd;

        public static Action OnPrePlayback;
        public static Action OnPreRecord;

        public static Action OnPlay;
        public static Action OnPause;
        public static Action OnStopPlayback;

        public static Action OnStartRecording;
        public static Action OnStopRecording;

        public static Action OnPlaybackTick;
        public static Action OnRecordTick;

        public static Action OnPreSnapshot;
    }
}
