using MelonLoader;
using SLZ.Rig;

using System.IO;

namespace NEP.MonoDirector
{
    public static class Constants
    {
        public static RigManager rigManager => BoneLib.Player.rigManager;

        public static readonly string dirBase = Path.Combine(MelonUtils.UserDataDirectory, "Not Enough Photons");
        public static readonly string dirMod = Path.Combine(dirBase, "MonoDirector");
        public static readonly string dirImg = Path.Combine(dirMod, "Images/");
        public static readonly string dirSFX = Path.Combine(dirMod, "SFX/");
    }
}
