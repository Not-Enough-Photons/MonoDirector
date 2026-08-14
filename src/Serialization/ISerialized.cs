namespace NEP.MonoDirector.Serialization;

public interface ISerialized
{
    public byte[] Serialize();
    public void Deserialize(Stream stream);
}