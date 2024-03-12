using System.Collections.Generic;

namespace NEP.MonoDirector
{
    public class Film
    {
        public Film()
        {
            Instance = this;
        }

        public static Film Instance { get; private set; }

        public Matinee ActiveScene { get; private set; }

        public float Runtime { get; private set; }

        public List<Matinee> Scenes => scenes;

        private List<Matinee> scenes = new List<Matinee>();

        public bool HasScenes()
        {
            return scenes.Count > 0;
        }

        public bool HasActiveScene()
        {
            return ActiveScene != null;
        }

        public void AddScene(Matinee scene)
        {
            scenes.Add(scene);
        }

        public void SetScene(Matinee scene)
        {
            ActiveScene = scene;
        }

        public void RemoveScene(Matinee scene)
        {
            scenes.Remove(scene);
        }
    }
}