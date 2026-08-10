using Il2CppSLZ.Marrow.Interaction;

namespace NEP.MonoDirector.Extensions;

public static class MarrowEntityExtensions
{
    public static void Freeze(this MarrowEntity entity)
    {
        foreach (var body in entity.Bodies)
            body._rigidbody.isKinematic = true;
    }

    public static void Unfreeze(this MarrowEntity entity)
    {
        foreach (var body in entity.Bodies)
            body._rigidbody.isKinematic = false;
    }
}