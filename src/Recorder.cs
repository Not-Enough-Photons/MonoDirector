using System.Collections;

using MelonLoader;
using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.State;

using UnityEngine;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Core;

public static class Recorder
{
    public static float RecordingTime { get => m_recordingTime; }
    public static float TakeTime;

    public static int RecordTick { get => m_recordTick; }

    public static int Countdown { get; private set; }

    public static List<Actor> ActiveActors = new List<Actor>();

    public static Actor ActiveActor { get => m_activeActor; }
    public static Actor LastActor { get => m_lastActor; }

    private static Actor m_activeActor;
    private static Actor m_lastActor;

    private static Coroutine m_recordRoute;

    private static float m_fpsTimer = 0f;

    private static float m_recordingTime;

    private static float m_timeSinceLastTick = 0;
    private static float m_timeSpentInMenu = 0f;
    private static bool m_usedMenu;

    private static int m_recordTick;

    public static void Initialize()
    {
        Events.OnPreRecord += OnPreRecord;
        Events.OnStartRecording += OnPostRecord;
        Events.OnRecordTick += OnRecordTick;
        Events.OnStopRecording += OnStopRecording;
    }

    public static void Shutdown()
    {
        Events.OnPreRecord -= OnPreRecord;
        Events.OnStartRecording -= OnPostRecord;
        Events.OnRecordTick -= OnRecordTick;
        Events.OnStopRecording -= OnStopRecording;
    }
    
    public static void SetActor(MarrowAvatar avatar)
    {
        m_lastActor = m_activeActor;
        m_activeActor = new Actor(avatar);
    }

    public static void SetUsedMenu(bool usedMenu)
    {
        m_usedMenu = usedMenu;
    }

    public static void Tick()
    {
        if (Director.PlayState != PlayState.Recording)
        {
            return;
        }

        try
        {
            Events.OnRecordTick?.Invoke();
        }
        catch (Exception e)
        {
            Logging.ErrorDebug(e.ToString());
            Bootstrap.AnnounceError();
        }
    }

    public static void StartRecordRoutine()
    {
        if (m_recordRoute == null)
        {
            m_recordRoute = MelonCoroutines.Start(RecordRoutine()) as Coroutine;
        }
    }

    public static void RecordCamera()
    {
        foreach (var castMember in Caster.Cast)
        {
            castMember?.Perform();
        }
    }

    public static void RecordActor()
    {
        try
        {
            if (Settings.World.recordActors)
            {
                m_activeActor.Record();
            }

            foreach (var prop in Caster.RecordProps)
            {
                prop.Record();
            }

            foreach (var castMember in Caster.Cast)
            {
                Playback.AnimateActor(castMember);
            }

            foreach (var prop in Caster.Props)
            {
                Playback.AnimateProp(prop);
            }
        }
        catch (Exception e)
        {
            Logging.ErrorDebug(e.ToString());
            Bootstrap.AnnounceError();
        }
        
    }

    /// <summary>
    /// Called when we first hit the record button.
    /// </summary>
    public static void OnPreRecord()
    {
        try
        {
            if (m_recordTick > 0)
            {
                m_recordTick = 0;
            }

            Playback.ResetPlayhead();

            m_fpsTimer = 0f;

            m_recordingTime = 0f;

            m_timeSpentInMenu = 0f;
            m_usedMenu = false;

            if (Settings.World.recordActors)
            {
                SetActor(Constants.RigManager.avatar);
            }

            foreach (var castMember in Caster.Cast)
            {
                castMember.OnSceneBegin();
            }

            foreach (var prop in Caster.Props)
            {
                prop.OnSceneBegin();
            }
        }
        catch (Exception e)
        {
            Logging.ErrorDebug(e.ToString());
            Bootstrap.AnnounceError();
        }
    }

    /// <summary>
    /// Called the moment the recording begins.
    /// </summary>
    public static void OnPostRecord()
    {
        try
        {
            m_activeActor?.Microphone?.SetCorrectionMode(Audio.ActorSpeech.AudioCorrectionMode.Corrected);
            m_activeActor?.Microphone?.RecordMicrophone();

            foreach (Actor actor in Caster.Cast)
            {
                actor.Microphone?.Playback();

                if (actor.Hidden)
                    actor.Hide();
            }
        }
        catch (Exception e)
        {
            Logging.ErrorDebug(e.ToString());
            Bootstrap.AnnounceError();
        }
        
    }

    /// <summary>
    /// Called every time a frame is recorded
    /// </summary>
    public static void OnRecordTick()
    {
        if (Director.PlayState == PlayState.Paused)
        {
            return;
        }

        try
        {
            m_recordTick++;
            m_recordingTime += m_timeSinceLastTick;

            if (m_usedMenu)
            {
                m_timeSpentInMenu += m_timeSinceLastTick;
            }

            // keep up!
            if (m_recordingTime > TakeTime)
            {
                TakeTime = m_recordingTime;
            }

            Playback.MovePlayhead(m_timeSinceLastTick);

            if (Director.CaptureState == CaptureState.CaptureCamera)
            {
                RecordCamera();
            }

            if (Director.CaptureState == CaptureState.CaptureActor)
            {
                RecordActor();
            }

            foreach (var castMember in Caster.Cast)
            {
                if (castMember != null)
                {
                    castMember.Perform();
                }
            }
        }
        catch(Exception e)
        {
            Logging.ErrorDebug(e.ToString());
            Bootstrap.AnnounceError();
        }
        
    }

    /// <summary>
    /// Called when the recording stops.
    /// </summary>
    public static void OnStopRecording()
    {
        try
        {
            m_recordingTime -= m_timeSpentInMenu - 1f;

            Director.ActiveScene.SetDuration(m_recordingTime);

            m_activeActor?.Microphone?.StopRecording();

            foreach (Actor actor in Caster.Cast)
            {
                actor.Microphone?.StopPlayback();

                if (actor.Hidden)
                    actor.Show();
            }

            if (Settings.World.recordActors)
            {
                m_activeActor.CreateProxy(Constants.Avatar);
                m_activeActor.UpdateClone();

                foreach (var recordedProp in Caster.RecordProps)
                {
                    m_activeActor.OwnProp(recordedProp);
                }

                // NOTE: Perhaps add the active actor to the list?
                Caster.CastActor(m_activeActor);
                Director.ActiveScene.AddActor(m_activeActor);
            }

            m_lastActor = m_activeActor;

            m_activeActor = null;

            Caster.CastActors(ActiveActors);
            Director.ActiveScene.AddActors(ActiveActors);
            Director.ActiveScene.AddProps(Caster.RecordProps.ToList());

            // Caster.AddProps(Director.RecordingProps);
            // Director.LastRecordedProps = Director.RecordingProps;

            Caster.TransferRecordedProps();

            ActiveActors.Clear();

            Director.ActiveScene.SetDuration(m_recordingTime);

            if (m_recordRoute != null)
            {
                MelonCoroutines.Stop(m_recordRoute);
                m_recordRoute = null;
            }
        }
        catch(Exception e)
        {
            Logging.ErrorDebug(e.ToString());
            Bootstrap.AnnounceError();
        }
    }

    public static IEnumerator RecordRoutine()
    {
        Events.OnPreRecord?.Invoke();

        for (Countdown = 0; Countdown < Settings.World.delay; Countdown++)
        {
            Events.OnTimerCountdown?.Invoke();
            yield return new WaitForSeconds(1);
        }

        FeedbackSFX.BeepHigh();

        Events.OnStartRecording?.Invoke();
        
        float perTick = 1.0F / Settings.World.fps;
        
        while (Director.PlayState == PlayState.Recording || Director.PlayState == PlayState.Paused)
        {
            // These are different for a reason!
            // virtual FPS and real FPS are decoupled here when asked!
            // Recording 15FPS with normal delta time means 15FPS becomes 5FPS in real frames!
            
            // Ignoring slomo means using deltaTime to store our recorded time
            // Therefore data is scaled with timescale
            if (Settings.World.ignoreSlomo) 
                m_timeSinceLastTick += Time.deltaTime;
            else
                m_timeSinceLastTick += Time.unscaledDeltaTime;
            
            // Temporal scaling increases the resolution when changing timescale
            if (Settings.World.temporalScaling) 
                m_fpsTimer += Time.unscaledDeltaTime;
            else
                m_fpsTimer += Time.deltaTime;

            if (m_fpsTimer > perTick)
            {
                Tick();
                m_fpsTimer = 0f;
                m_timeSinceLastTick = 0;
            }

            yield return null;
        }

        Director.Stop();
        Events.OnStopRecording?.Invoke();
        yield return null;
    }
}
