namespace NEP.MonoDirector.Events;

public abstract class EventPacket
{
    protected EventPacket() { }
    
    public virtual byte ID => 0xFF;

    public bool OneShot => m_oneShot;
    public bool Executed => m_executed;

    private bool m_oneShot;
    private bool m_executed;

    public abstract void Execute();
    public abstract byte[] Serialize();
    public abstract void Deserialize(Stream stream);

    public void SetOneShot(bool oneshot)
    {
        m_oneShot = oneshot;
    }

    public void MarkExecuted()
    {
        m_executed = true;
    }

    public void Reset()
    {
        m_executed = false;
    }
}