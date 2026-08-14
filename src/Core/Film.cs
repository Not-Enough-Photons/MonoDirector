using System.Text;
using NEP.MonoDirector.Serialization;

namespace NEP.MonoDirector.Core;

public sealed class Film : ISerialized
{
    public Film()
    {
        m_name = "Film";
        m_scenes = new List<Scene>();
        m_levelBarcode = string.Empty;
    }

    public Film(List<Scene> scenes)
    {
        m_name = "Film";
        m_scenes = scenes;
        m_levelBarcode = string.Empty;
    }

    public const byte Version = 1;
    
    public string Name => m_name;
    public float Runtime => m_runtime;
    public bool Empty => Scenes.Count == 0;
    public string LevelBarcode => m_levelBarcode;
    public IReadOnlyList<Scene> Scenes => m_scenes.AsReadOnly();
    
    private string m_name;
    private float m_runtime;
    private string m_levelBarcode;
    private List<Scene> m_scenes;

    public void AddScene(Scene scene)
    {
        scene.SetIndex(m_scenes.Count);
        m_scenes.Add(scene);
        m_runtime += scene.Duration;
    }

    public void RemoveScene(Scene scene)
    {
        m_scenes.Remove(scene);
        m_runtime -= scene.Duration;

        if (m_runtime <= 0f)
            m_runtime = 0f;

        for (int i = 0; i < m_scenes.Count; i++)
            m_scenes[i].SetIndex(i);
    }

    public void SetLevel(string barcode)
    {
        m_levelBarcode = barcode;
    }

    public byte[] Serialize()
    {
        throw new NotImplementedException();
    }

    public void Deserialize(Stream stream)
    {
        throw new NotImplementedException();
    }
}