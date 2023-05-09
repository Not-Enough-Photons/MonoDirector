﻿namespace NEP.MonoDirector
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
            public static bool useMicrophone = false;
            public static bool micPlayback = false;
            public static float playbackRate = 1f;
        }

        public static class Debug
        {
            public static bool debugEnabled = true;
            public static bool useKeys = true;
        }
    }
}
