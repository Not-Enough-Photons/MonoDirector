using UnityEngine;

namespace NEP.MonoDirector.Extensions;

public static class TransformExtensions
{
    public static void LookAtYAxis(this Transform transform, Transform target, float offset = 0f)
    {
        transform.LookAtYAxis(target.position, offset);
    }

    public static void LookAtYAxis(this Transform transform, Vector3 target, float offset = 0f)
    {
        Vector3 lookRotation = Quaternion.LookRotation(target - transform.position).eulerAngles;
        Quaternion yRotation = Quaternion.Euler(0f, lookRotation.y + offset, 0f);
        transform.rotation = yRotation;
    }
}