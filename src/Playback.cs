using System;
using System.Collections;
using MelonLoader;

using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.State;

using UnityEngine;

namespace NEP.MonoDirector.Core;

public static class Playback
{
    /// <summary>
    /// The current time stamp of the playhead
    /// </summary>
    public static float PlaybackTime => m_playbackTime;
    
    /// <summary>
    /// The rate at which the playhead seeks, similar to Time.timeScale
    /// </summary>
    public static float PlaybackRate = 1;
    // TODO: Should this be private for some reason?

    public static int Countdown { get; private set; }

    private static float m_playbackTime;
    private static Coroutine m_playRoutine;

    private static Film m_film;
    private static Scene m_scene;

    public static void Initialize()
    {
        Events.OnPrePlayback += OnPrePlayback;
        Events.OnPlay += OnPlay;
        Events.OnPlaybackTick += OnPlaybackTick;
        Events.OnStopPlayback += OnStopPlayback;
    }

    public static void Shutdown()
    {
        Events.OnPrePlayback += OnPrePlayback;
        Events.OnPlay += OnPlay;
        Events.OnPlaybackTick += OnPlaybackTick;
        Events.OnStopPlayback += OnStopPlayback;
    }
    
    //
    // Playback modification methods
    //
    public static void ResetPlayhead() => m_playbackTime = 0f;

    public static void MovePlayhead(float amount) => m_playbackTime += amount;

    //
    // Playback methods
    //
    
    /// <summary>
    /// Called per frame, invokes any and all OnPlaybackTick delegates
    /// </summary>
    public static void Tick()
    {
        if (Director.PlayState != PlayState.Playing)
            return;

        try
        {
            Events.OnPlaybackTick?.Invoke();
        }
        catch(Exception e)
        {
            Logging.ErrorDebug(e.ToString());
            Bootstrap.AnnounceError();
        }
    }
    
    /// <summary>
    /// Called when playback is requested to start
    /// This spawns a coroutine that waits until a delay has passed to begin playing
    /// </summary>
    public static void BeginPlayback()
    {
        if (Director.LastPlayState == PlayState.Paused)
        {
            Director.SetPlayState(PlayState.Playing);
            return;
        }

        try
        {
            m_film = Director.ActiveFilm;
            m_scene = m_film.Scenes[0];

            if (m_playRoutine == null)
                m_playRoutine = MelonCoroutines.Start(PlayRoutine()) as Coroutine;
        }
        catch(Exception e)
        {
            Logging.ErrorDebug(e.ToString());
            Bootstrap.AnnounceError();
        }
    }

    /// <summary>
    /// Called before playback begins
    /// This resets the scene state and playhead
    /// </summary>
    public static void OnPrePlayback()
    {
        try
        {
            ResetPlayhead();

            foreach (var castMember in Caster.Cast)
            {
                castMember.OnSceneBegin();
            }

            foreach (var prop in Caster.Props)
            {
                prop.OnSceneBegin();
            }
        }
        catch(Exception e)
        {
            Logging.ErrorDebug(e.ToString());
            Bootstrap.AnnounceError();
        }
    }

    /// <summary>
    /// Called during playback
    /// </summary>
    public static void OnPlay()
    {
        try
        {
            foreach (var actor in Caster.Cast)
            {
                if (actor is Actor actorPlayer)
                {
                    actorPlayer?.Microphone?.StopPlayback();
                    actorPlayer?.Microphone?.Playback();

                    if (actorPlayer.Hidden)
                        actorPlayer.Hide();
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
    /// Called per playback tick
    /// </summary>
    public static void OnPlaybackTick()
    {
        if (Director.PlayState == PlayState.Stopped || Director.PlayState == PlayState.Paused)
        {
            return;
        }

        try
        {
            AnimateAll();
            m_playbackTime += PlaybackRate * Time.deltaTime;
        }
        catch(Exception e)
        {
            Logging.ErrorDebug(e.ToString());
            Bootstrap.AnnounceError();
        }
    }

    /// <summary>
    /// Called when playback is requested to stop
    /// </summary>
    public static void OnStopPlayback()
    {
        try
        {
            foreach (Actor actor in Caster.Cast)
            {
                actor.Microphone?.StopPlayback();

                if (actor.Hidden)
                    actor.Show();
            }

            if (m_playRoutine != null)
            {
                MelonCoroutines.Stop(m_playRoutine);
                m_playRoutine = null;
            }
        }
        catch(Exception e)
        {
            Logging.ErrorDebug(e.ToString());
            Bootstrap.AnnounceError();
        }
    }

    /// <summary>
    /// Manually seeks the playback head in the provided direction
    /// Negative seconds will reverse the playback 
    /// </summary>
    /// <param name="amount">The amount of seconds to seek the playback</param>
    public static void Seek(float amount)
    {
        if (Director.PlayState != PlayState.Stopped)
            return;

        if (m_playbackTime <= 0f)
            m_playbackTime = 0f;

        if (m_playbackTime >= m_scene.Duration)
            m_playbackTime = m_scene.Duration;

        AnimateAll();

        m_playbackTime += amount;
    }

    /// <summary>
    /// Animates all tracked scene objects
    /// Call when playback head is seeked to make sure changes are applied!
    /// </summary>
    public static void AnimateAll()
    {
        try
        {
            foreach (var actor in Caster.Cast)
                AnimateActor(actor);

            foreach (var prop in Caster.Props)
                AnimateProp(prop);
        }
        catch (Exception e)
        {
            Logging.ErrorDebug(e.ToString());
            Bootstrap.AnnounceError();
        }
    }
    
    /// <summary>
    /// Animates the provided actor
    /// </summary>
    /// <param name="actor">The actor to "act"</param>
    public static void AnimateActor(Actor actor)
    {
        if (actor != null)
            actor.Perform();
    }

    /// <summary>
    /// Animates the provided prop
    /// </summary>
    /// <param name="prop">The prop to "act"</param>
    public static void AnimateProp(Prop prop)
    {
        if (prop != null)
            prop.Perform();
    }

    /// TODO: Is PlayRoutine() having a delay necessary?
    
    /// <summary>
    /// Playback coroutine. Supports a delay of any duration.
    /// </summary>
    /// <returns></returns>
    public static IEnumerator PlayRoutine()
    {
        bool begun = false;

        for (int i = 0; i < m_film.Scenes.Count; i++)
        {
            Events.OnPrePlayback?.Invoke();

            if (Director.PlayState == PlayState.Stopped)
            {
                break;
            }

            m_scene = m_film.Scenes[i];
            Director.SetScene(m_scene);

            if (!begun)
            {
                for (Countdown = 0; Countdown < Settings.World.delay; Countdown++)
                {
                    Events.OnTimerCountdown?.Invoke();
                    yield return new WaitForSeconds(1);
                }

                begun = true;
            }

            FeedbackSFX.BeepHigh();

            Events.OnPlay?.Invoke();

            while (Director.PlayState == PlayState.Playing || Director.PlayState == PlayState.Paused)
            {
                // TODO: Replace this with WaitUntil to prevent Coroutine garbage?
                while (Director.PlayState == PlayState.Paused)
                    yield return null;

                if (PlaybackTime >= m_scene.Duration)
                    break;

                Tick();

                yield return null;
            }

            yield return null;
        }

        Director.Stop();
        Events.OnStopPlayback?.Invoke();
    }
}
