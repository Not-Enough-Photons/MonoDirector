using Newtonsoft.Json;

namespace NEP.MonoDirector.Content;

[JsonObject(MemberSerialization.Fields)]
public sealed class PlatformObject
{
    public string Platform => m_platform;
    public int FileID => m_fileID;

    [JsonProperty("platform")] private string m_platform;
    [JsonProperty("modfile_live")] private int m_fileID;
}
