namespace NEP.MonoDirector.Core.States;

public abstract class State
{
    public abstract void Start();
    public abstract void Update();
    public abstract void Stop();
}