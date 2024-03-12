namespace NEP.MonoDirector
{
    public class Take
    {
        public Take(Matinee scene)
        {
            Scene = scene;
        }

        public Matinee Scene { get; private set; }
    }
}