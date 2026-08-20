using NEP.MonoDirector.Serialization;

namespace NEP.MonoDirector.Keyframes;

public class Keyframe<T>
{
    public Keyframe()
    {
        m_time = 0f;
        m_value = default;
    }
	
    public Keyframe(float time, T value)
    {
        m_time = time;
        m_value = value;
    }

    public static Keyframe<T> EmptyFrame = new Keyframe<T>();
	
    public float Time => m_time;
    public T Value => m_value;
	
    private float m_time;
    private T m_value;
	
    public void SetTime(float time)
    {
        m_time = time;
    }
}