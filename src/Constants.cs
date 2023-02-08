using MelonLoader;
using SLZ.Rig;

namespace NEP.MonoDirector
{
    public static class Constants
    {
        public static RigManager rigManager => BoneLib.Player.rigManager;

        public static float maxTimeToRecord = 30f;

        public static int maxRecordingSize = 2048;

        public static readonly string dirSFX = MelonUtils.UserDataDirectory + "/Not Enough Photons/MonoDirector/SFX/";
    }
}
