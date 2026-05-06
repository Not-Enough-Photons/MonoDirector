namespace NEP.MonoDirector.Core;

public sealed class Film
{
    public Film()
    {
        m_scenes = new List<Scene>();
    }

    public Film(List<Scene> scenes)
    {
        m_scenes = scenes;
    }

    public IReadOnlyList<Scene> Scenes => m_scenes.AsReadOnly();
    public string Name => m_name;
    public float Runtime => m_runtime;
    public bool Empty => Scenes.Count == 0;

    private List<Scene> m_scenes;
    private string m_name;
    private float m_runtime;

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

        for (int i = 0; i < m_scenes.Count; i++)
            m_scenes[i].SetIndex(i);
    }
}
