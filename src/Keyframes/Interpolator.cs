using NEP.MonoDirector.Events;
using UnityEngine;

using NEP.MonoDirector.Keyframes;

namespace NEP.MonoDirector.Content;

public static class Interpolator
{
    public static float GetDelta(float time, float next, float previous)
    {
        float gap = next - previous;
        float head = time - previous;
        return head / gap;
    }

    public static Vector3 EvaluatePosition(float time, ref KeyframeTrack<Vector3> track)
    {
        Keyframe<Vector3> previous = new Keyframe<Vector3>();
        Keyframe<Vector3> next = new Keyframe<Vector3>();

        foreach (var frame in track.Frames)
        {
            previous = next;
            next = frame;

            if (frame.Time > time)
                break;
        }

        float delta = GetDelta(time, next.Time, previous.Time);

        return Vector3.Lerp(previous.Value, next.Value, delta);
    }

    public static Quaternion EvaluateRotation(float time, ref KeyframeTrack<Quaternion> track)
    {
        Keyframe<Quaternion> previous = new Keyframe<Quaternion>();
        Keyframe<Quaternion> next = new Keyframe<Quaternion>();

        foreach (var frame in track.Frames)
        {
            previous = next;
            next = frame;

            if (frame.Time > time)
                break;
        }

        float delta = GetDelta(time, next.Time, previous.Time);

        return Quaternion.Slerp(previous.Value, next.Value, delta);
    }
}