using HarmonyLib;
using NEP.MonoDirector;
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using SLZ.Rig;
using static MelonLoader.MelonLogger;

public static class Seat
{
    [HarmonyPatch(typeof(SLZ.Vehicle.Seat))]
    [HarmonyPatch(nameof(SLZ.Vehicle.Seat.Register))]
    public static class Register
    {
        public static void Postfix(SLZ.Vehicle.Seat __instance, RigManager rM)
        {
            Main.Logger.Msg("Register Rig");
            Actor activeActor = Recorder.instance.ActiveActor;

            if(activeActor == null)
            {
                return;
            }

            activeActor.CaptureAvatarAction(Recorder.instance.RecordTick, () => activeActor.ParentToSeat(__instance));
        }
    }

    [HarmonyPatch(typeof(SLZ.Vehicle.Seat))]
    [HarmonyPatch(nameof(SLZ.Vehicle.Seat.DeRegister))]
    public static class DeRegister
    {
        public static void Postfix()
        {
            Main.Logger.Msg("Deregister Rig");

            Actor activeActor = Recorder.instance.ActiveActor;

            if (activeActor == null)
            {
                return;
            }

            activeActor.CaptureAvatarAction(Recorder.instance.RecordTick, () => activeActor.UnparentSeat());
        }
    }
}