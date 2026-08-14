using Newtonsoft.Json;

namespace NEP.MonoDirector.Content;

[JsonObject(MemberSerialization.Fields)]
public class DataResult
{
    public ModObject[] Data => m_data;

    [JsonProperty("data")] private ModObject[] m_data;
}
