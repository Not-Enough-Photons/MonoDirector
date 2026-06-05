namespace NEP.MonoDirector.Archetypes;

public enum ActionType : byte
{
    // Generic
    Hide,
    Show,
    GripEventAttached,
    GripEventDetached,
    GripEventIndexDown,
    GripEventMenuTap,
    
    // Gun
    Fire,
    SlideGrabbed,
    SlideReleased,
    SlidePulled,
    SlideUpdate,
    SlideReturned,
    AmmoChanged,
    InsertMag,
    RemoveMag,
    AnimStateUpdate,
    UpdateArt
}