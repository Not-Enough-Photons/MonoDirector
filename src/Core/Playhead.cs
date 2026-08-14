namespace NEP.MonoDirector.Core;

public static class Playhead
{
    public static float Time => m_time;
    public static float Duration => m_duration;
    public static float Rate => m_rate;
    
    public static bool Finished => m_finished;
    public static bool Looping => m_loop;

    public static float PerTick => 1f / Settings.FPS;

    private static float m_time;
    private static float m_duration;
    private static float m_rate = 1f;
    
    private static bool m_limit = true;
    private static bool m_loop = false;
    private static bool m_finished = false;

    public static void Initialize()
    {
        m_time = 0f;
        m_duration = 0f;
    }

    public static void Advance(float step)
    {
        m_time += step * m_rate;

        if (m_time >= m_duration)
        {
            if (m_limit)
                m_finished = true;
            else
                m_duration = m_time;
        }

        if (m_time <= 0f)
            m_time = 0f;
    }

    public static void Reset()
    {
        m_time = 0f;
        m_finished = false;
    }

    public static void SetDuration(float duration)
    {
        if (duration <= 0f)
            duration = 0f;

        m_duration = duration;
    }

    public static void UseLimit(bool limit)
    {
        m_limit = limit;
    }

    public static void UseLoop(bool loop)
    {
        m_loop = true;
    }

    public static void SetPlaybackRate(float rate)
    {
        m_rate = rate;
    }
}