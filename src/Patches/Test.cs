using MelonLoader;
using NEP.MonoDirector;
using UnityExplorer.ObjectExplorer;

[HarmonyLib.HarmonyPatch(typeof(SceneHandler), "Update")]
public static class UnityExplorerCrashDebugPatches
{
    public static void Postfix()
    {
        NEP.MonoDirector.Main.Logger.Msg("SceneExplorer::UpdateTree");
    }
}