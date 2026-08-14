using NEP.MonoDirector.Content;
using UnityEngine;

namespace NEP.MonoDirector.Core.States;

public sealed class CountdownState : State
{
    private int m_counts;
    private State m_nextState;

    private int m_currentCount;
    private float m_time;
    
    public override void Start()
    {
        m_time = 0f;
        m_currentCount = 0;
        
        if (m_counts == 0)
        {
            if (m_nextState != null)
                Director.SwitchTo(m_nextState);

            return;
        }
        
        Director.PlaySound(BundleLoader.BeepClip);
    }

    public override void Update()
    {
        m_time += Time.deltaTime;

        if (m_time >= 1f)
        {
            m_currentCount++;
            m_time = 0f;
            
            if (m_currentCount != m_counts)
                Director.PlaySound(BundleLoader.BeepClip);
        }

        if (m_currentCount == m_counts)
        {
            m_currentCount = 0;
            Director.SwitchTo(m_nextState);
            Director.PlaySound(BundleLoader.LinkAudioClip);
        }
    }

    public override void Stop()
    {
        
    }

    public void SetCounts(int counts)
    {
        if (counts <= 0)
            counts = 0;

        m_counts = counts;
    }

    public void SetNextState(State state)
    {
        m_nextState = state;
    }
}