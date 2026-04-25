using BoneLib;
using UnityEngine;
using System.Reflection;

namespace NEP.MonoDirector.Data
{
    public static class BundleLoader
    {
        public static GameObject FrameObject { get => m_frameObject; }
        public static GameObject PropMarkerObject { get => m_propMarkerObject; }
        public static GameObject MenuButtonObject { get => m_menuButtonObject; }
        public static Texture2D IconProp { get  => m_iconProp; }
        public static Texture2D IconHidden { get => m_iconHidden; }
        public static Texture2D IconVisible { get => m_iconVisible; }
        public static Texture2D IconGun { get => m_iconGun; }
        public static Texture2D IconMagazine { get => m_iconMagazine; }
        public static Texture2D IconVehicle { get => m_iconVehicle; }

        public static AudioClip BeepClip { get => m_beepClip; }
        public static AudioClip PreRollClip { get => m_preRollClip; }
        public static AudioClip PostRollClip { get => m_postRollClip; }
        public static AudioClip LinkAudioClip { get => m_linkAudioClip; }
        public static AudioClip ErrorClip { get => m_errorClip; }

        private static AssetBundle m_uiBundle;
        private static AssetBundle m_soundBundle;

        private static GameObject m_frameObject;
        private static GameObject m_propMarkerObject;
        private static GameObject m_menuButtonObject;

        private static AudioClip m_beepClip;
        private static AudioClip m_preRollClip;
        private static AudioClip m_postRollClip;
        private static AudioClip m_linkAudioClip;
        private static AudioClip m_errorClip;

        private static Texture2D m_iconProp;
        private static Texture2D m_iconHidden;
        private static Texture2D m_iconVisible;
        private static Texture2D m_iconGun;
        private static Texture2D m_iconMagazine;
        private static Texture2D m_iconVehicle;

        public static void Initialize()
        {
            m_uiBundle = LoadBundle("ui");
            m_soundBundle = LoadBundle("sounds");

            m_frameObject = m_uiBundle.LoadPersistentAsset<GameObject>("Frame");
            m_propMarkerObject = m_uiBundle.LoadPersistentAsset<GameObject>("PropMarker");
            m_menuButtonObject = m_uiBundle.LoadPersistentAsset<GameObject>("MonoDirectorButton");
            m_iconProp = m_uiBundle.LoadPersistentAsset<Texture2D>("Icon_Prop");
            m_iconHidden = m_uiBundle.LoadPersistentAsset<Texture2D>("Icon_Hidden");
            m_iconVisible = m_uiBundle.LoadPersistentAsset<Texture2D>("Icon_Visible");
            m_iconGun = m_uiBundle.LoadPersistentAsset<Texture2D>("Icon_Gun");
            m_iconMagazine = m_uiBundle.LoadPersistentAsset<Texture2D>("Icon_Magazine");
            m_iconVehicle = m_uiBundle.LoadPersistentAsset<Texture2D>("Icon_Vehicle");

            m_beepClip = m_soundBundle.LoadPersistentAsset<AudioClip>("beep");
            m_preRollClip = m_soundBundle.LoadPersistentAsset<AudioClip>("preroll");
            m_postRollClip = m_soundBundle.LoadPersistentAsset<AudioClip>("postroll");
            m_linkAudioClip = m_soundBundle.LoadPersistentAsset<AudioClip>("linkaudio");
            m_errorClip = m_soundBundle.LoadPersistentAsset<AudioClip>("kl_fiddlesticks");

        }

        private static AssetBundle LoadBundle(string name)
        {
            string resourcePath = "NEP.MonoDirector.Resources";

            if (!HelperMethods.IsAndroid())
            {
                resourcePath += $".md_{name}_pc.bundle";
            }
            else
            {
                resourcePath += $".md_{name}_quest.bundle";
            }

            return HelperMethods.LoadEmbeddedAssetBundle(Assembly.GetExecutingAssembly(), resourcePath);
        }
    }
}
