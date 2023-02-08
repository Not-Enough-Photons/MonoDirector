namespace NEP.MonoDirector
{
    public static class Settings
    {
        public static class Camera
        {
            public static float cameraSpeedSlow { get; set; }
            public static float cameraSpeedFast { get; set; }
            public static float cameraSmoothRotationDelta { get; set; }
        }

        public static class World
        {
            public static bool spawnGunProps = false;
        }
    }
}
