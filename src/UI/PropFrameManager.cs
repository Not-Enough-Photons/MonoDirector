using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Pool;
using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.Archetypes.Proxy;
using NEP.MonoDirector.State;
using UnityEngine;

namespace NEP.MonoDirector.UI;

public static class PropFrameManager
{
    private static GameObject container;
    private static Dictionary<Prop, GameObject> frames;
    private static List<GameObject> loadedFrameObjects;
    private static List<GameObject> activeFrames;

    public static void Initialize()
    {
        frames = new Dictionary<Prop, GameObject>();
        loadedFrameObjects = new List<GameObject>();
        activeFrames = new List<GameObject>();

        container = new GameObject("[MonoDirector] - Prop Frame Container");
        container.transform.SetParent(Bootstrap.MainContainerObject.transform);

        for (int i = 0; i < 32; i++)
        {
            GameObject obj = GameObject.Instantiate(BundleLoader.FrameObject);
            obj.SetActive(false);
            obj.transform.SetParent(container.transform);
            obj.transform.localPosition = Vector3.zero;
            loadedFrameObjects.Add(obj);
        }

        Events.OnPlayStateSet += OnPlayStateSet;
    }

    public static void CleanUp()
    {
        frames.Clear();
        loadedFrameObjects.Clear();
        activeFrames.Clear();
    }

    public static void OnPlayStateSet(PlayState state)
    {
        if (state == PlayState.Stopped)
            ShowFrames();
        else
            HideFrames();
    }

    public static GameObject AddFrameToProp(Prop prop)
    {
        if (frames.ContainsKey(prop))
            return null;

        GameObject asset = loadedFrameObjects.FirstOrDefault((frame) => !activeFrames.Contains(frame));

        asset.gameObject.SetActive(true);

        MarrowBody body = prop.Proxy.Entity.Bodies.First();
        
        asset.transform.SetParent(prop.Proxy.transform);
        asset.transform.localPosition = body.Bounds.center;
        asset.transform.localRotation = Quaternion.identity;
        asset.transform.localScale = body.Bounds.extents;

        frames.Add(prop, asset);
        activeFrames.Add(asset);

        return asset;
    }

    public static void RemoveFrameFromProp(Prop prop)
    {
        if (!frames.ContainsKey(prop))
            return;

        GameObject frame = frames[prop];
        frame.gameObject.SetActive(false);
        frame.transform.parent = container.transform;
        frame.transform.localScale = Vector3.one;
        frames.Remove(prop);
        activeFrames.Remove(frame.gameObject);
    }

    public static void ShowFrames()
    {
        foreach (var frame in activeFrames)
            frame.SetActive(true);
    }

    public static void HideFrames()
    {
        foreach (var frame in activeFrames)
            frame.SetActive(false);
    }

    internal static void OnFrameSpawned(GameObject frameObject)
    {
        frameObject.SetActive(false);
        frameObject.transform.SetParent(container.transform);
        loadedFrameObjects.Add(frameObject);
    }
}
