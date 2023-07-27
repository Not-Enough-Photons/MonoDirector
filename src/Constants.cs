using MelonLoader;
using SLZ.Rig;

using System.IO;

namespace NEP.MonoDirector
{
    public static class Constants
    {
        public static RigManager RigManager => BoneLib.Player.rigManager;

        public static readonly string DirBase = Path.Combine(MelonUtils.UserDataDirectory, "Not Enough Photons");
        public static readonly string DirMod = Path.Combine(DirBase, "MonoDirector");
        public static readonly string DirImg = Path.Combine(DirMod, "Images/");
        public static readonly string DirSFX = Path.Combine(DirMod, "SFX/");

        public static readonly string[] AutoRegisterDirs = new string[]
        {
            DirBase,
            DirMod,
            DirImg,
            DirSFX
        };
    }
}
