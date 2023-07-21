namespace NEP.MonoDirector.Data
{
    public interface IJSONData
    {
        string ToJSON();
        void FromJSON(string json);
    }
}