using NEP.MonoDirector.Serialization;

namespace NEP.MonoDirector.Keyframes;

public sealed class KeyframeTrack<T>
{
    public KeyframeTrack()
    {
        m_name = string.Empty;
        m_time = 0f;
        m_frames = new List<Keyframe<T>>();
    }
	
    public KeyframeTrack(string name)
    {
        m_name = name;
        m_time = 0f;
        m_frames = new List<Keyframe<T>>();
    }
	
    public Keyframe<T> this[int key]
    {
        get => m_frames[key];
        set => m_frames[key] = value;
    }
	
    public string Name => m_name;
    public float Time => m_time;
    public IReadOnlyList<Keyframe<T>> Frames => m_frames.AsReadOnly();

    public Keyframe<T> FirstFrame
    {
        get
        {
            if (m_frames.Count == 0)
                return Keyframe<T>.EmptyFrame;
            
            return m_frames[0];
        }
    }

    public Keyframe<T> LastFrame
    {
        get
        {
            if (m_frames.Count == 0)
                return Keyframe<T>.EmptyFrame;

            return m_frames[^(m_frames.Count - 1)];
        }
    }
	
    private string m_name;
    private List<Keyframe<T>> m_frames;
    private float m_time;
	
    public void Add(float time, T value)
    {
        m_frames.Add(new Keyframe<T>(time, value));
    }
	
    public void Remove(Keyframe<T> frame)
    {
        m_frames.Remove(frame);
    }
	
    public void SetTime(float time)
    {
        foreach (var frame in m_frames)
            frame.SetTime(time);
    }
}