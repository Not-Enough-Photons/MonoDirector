namespace NEP.MonoDirector.Keyframes;

public static class Trimmer
{
    public static List<Keyframe<T>> GetFrames<T>(float start, float end, ref KeyframeTrack<T> track)
    {
        List<Keyframe<T>> keyframes = new List<Keyframe<T>>();

        foreach (var frame in track.Frames)
        {
            if (frame.Time >= start && frame.Time <= end)
                keyframes.Add(frame);
        }

        return keyframes;
    }
}