namespace NEP.MonoDirector.Core.States;

public abstract class State
{
    public abstract void Start();
    public abstract void Update(float time);
    public abstract void Stop();
}