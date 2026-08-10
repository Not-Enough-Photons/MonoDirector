using MelonLoader;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector;

public static class BuildInfo
{
    public const string Name = "MonoDirector";
    public const string Description = "A movie/photo making utility for BONELAB!";
    public const string Author = "Not Enough Photons";
    public const string Company = "Not Enough Photons";
    public const string Version = "1.3.0";
    public const string DownloadLink = "https://bonelab.thunderstore.io/c/bonelab/p/NotEnoughPhotons/MonoDirector";
}

public class Main : MelonMod
{
    public override void OnInitializeMelon() => Engine.Initialize();

    public override void OnUpdate() => Engine.Update();

    public override void OnDeinitializeMelon() => Engine.Shutdown();
}
