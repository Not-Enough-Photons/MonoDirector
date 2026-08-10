using NEP.MonoDirector.Core;

using UnityEngine;

using BoneLib.BoneMenu;

namespace NEP.MonoDirector.UI;

internal static class MDBoneMenu
{
    public static Page MonoDirectorPage => m_monoDirectorPage;
    public static Page PlaybackPage => m_playbackPage;
    public static Page SettingsPage => m_settingsPage;

    internal static Page m_modPage;

    internal static Page m_monoDirectorPage;

    internal static Page m_playbackPage;
    internal static Page m_actorCategory;
    internal static Page m_settingsPage;

    internal static void Initialize()
    {
        m_modPage = Page.Root.CreatePage("Not Enough Photons", Color.white);

        m_monoDirectorPage = m_modPage.CreatePage("Mono<color=red>Director</color>", Color.white);

        m_playbackPage = m_monoDirectorPage.CreatePage("Playback", Color.white);
        m_actorCategory = m_monoDirectorPage.CreatePage("Actors", Color.white);
        m_settingsPage = m_monoDirectorPage.CreatePage("Settings", Color.white);

        BuildPlaybackMenu(m_playbackPage);
    }

    private static void BuildPlaybackMenu(Page category)
    {
        category.CreateFunction(
            "Record",
            Color.red,
            Director.Record
        );

        category.CreateFunction(
            "Play",
            Color.green,
            Director.Play
        );
        
        category.CreateFunction(
            "Pause",
            Color.yellow,
            Director.Pause
        );

        category.CreateFunction(
            "Stop",
            Color.red,
            Director.Stop
        );
    }
}