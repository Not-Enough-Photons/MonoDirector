using BoneLib;
using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.Warehouse;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.Archetypes.Proxy;
using NEP.MonoDirector.State;
using UnityEngine;

namespace NEP.MonoDirector.UI;

public static class ActorFrameManager
{
    private static GameObject m_container;
    private static Dictionary<ActorProxy, ActorFrame> m_frames;
    private static List<ActorFrame> m_loadedFrames;
    private static List<ActorFrame> m_activeFrames;

    public static void Initialize()
    {
        m_frames = new Dictionary<ActorProxy, ActorFrame>();
        m_loadedFrames = new List<ActorFrame>();
        m_activeFrames = new List<ActorFrame>();

        m_container = new GameObject("[MonoDirector] - Actor Frame Container");
        m_container.transform.SetParent(Bootstrap.MainContainerObject.transform);

        for (int i = 0; i < 32; i++)
        {
            GameObject obj = GameObject.Instantiate(BundleLoader.FrameObject, m_container.transform);
            obj.SetActive(false);
            obj.transform.localPosition = Vector3.zero;
            ActorFrame frame = new ActorFrame(obj);
            frame.Hide();
            m_loadedFrames.Add(frame);
        }

        Events.OnPlayStateSet += ShowFrames;
    }

    public static void CleanUp()
    {
        Events.OnPlayStateSet -= ShowFrames;

        m_frames.Clear();
        m_loadedFrames.Clear();
        m_activeFrames.Clear();
    }
    
    public static void Update()
    {
        if (m_loadedFrames == null)
            return;
        
        foreach (var marker in m_loadedFrames)
        {
            if (marker.Active)
                marker.Update();
        }
    }

    public static ActorFrame AddFrameToActor(ActorProxy proxy)
    {
        if (m_frames.ContainsKey(proxy))
            return null;

        ActorFrame frame = m_loadedFrames.FirstOrDefault((frame) => !frame.Active);

        if (frame == null)
            return null;
        
        frame.Parent(proxy);
        frame.SetTarget(proxy.Collider.transform);
        frame.Show();
        
        m_frames.Add(proxy, frame);
        m_activeFrames.Add(frame);
        return frame;
    }

    public static void RemoveFrameFromActor(ActorProxy proxy)
    {
        if (!m_frames.TryGetValue(proxy, out ActorFrame frame))
            return;

        frame = m_frames[proxy];
        frame.SetOffset(Vector3.zero);
        frame.SetTarget(null);
        frame.Hide();
        m_frames.Remove(proxy);
        m_activeFrames.Remove(frame);
    }

    private static void ShowFrames(PlayState playState)
    {
        if (Caster.SelectedActors.Count == 0)
            return;
        
        if (playState == PlayState.Preplaying || playState == PlayState.Prerecording)
        {
            foreach (var frame in m_activeFrames)
                frame.Hide();
        }

        if (playState == PlayState.Stopped)
        {
            foreach (var frame in m_activeFrames)
                frame.Show();
        }
    }
}
