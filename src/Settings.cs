namespace NEP.MonoDirector
{
    public static class Settings
    {
        public static class Camera
        {
            public static float cameraSpeedSlow;
            public static float cameraSpeedFast;
            public static float cameraSmoothRotationDelta;
            public static bool useHeadCamera;
        }

        public static class World
        {
            public static bool spawnGunProps = false;
            public static bool spawnGunNPCs = false;
        }

        public static class Debug
        {
            public static bool debugEnabled;
            public static bool useKeys;
        }
    }
}
