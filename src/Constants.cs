using MelonLoader;
using SLZ.Rig;

namespace NEP.MonoDirector
{
    public static class Constants
    {
        public static RigManager rigManager => BoneLib.Player.rigManager;

        public static readonly string dirSFX = MelonUtils.UserDataDirectory + "/Not Enough Photons/MonoDirector/SFX/";
    }
}
