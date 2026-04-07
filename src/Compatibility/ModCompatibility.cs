using MelonLoader;

namespace NEP.MonoDirector.Compatibility
{
    public static class ModCompatibility
    {
        public static bool HasWideEye => m_hasWideEye;

        private static bool m_hasWideEye;

        public static void Scan()
        {
            m_hasWideEye = MelonMod.FindMelon("WideEye", "HL2H0") != null;
        }
    }
}
