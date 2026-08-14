using Il2CppSLZ.Marrow.Interaction;

using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Content;

using UnityEngine;

using Object = UnityEngine.Object;

namespace NEP.MonoDirector.Visuals;

public static class VisualManager
{
    private const int MaxBoundingBoxes = 32;
    private const int MaxMarkers = 64;
    
    private static List<VisBounds> m_bounds;
    private static List<VisMarker> m_markers;

    private static Dictionary<Archetype, VisBounds> m_attachedBounds;
    private static Dictionary<Archetype, VisMarker> m_attachedMarkers;

    private static GameObject m_boundsContainer;
    private static GameObject m_markerContainer;
    
    public static void Initialize()
    {
        m_bounds = new List<VisBounds>();
        m_markers = new List<VisMarker>();

        m_attachedBounds = new Dictionary<Archetype, VisBounds>();
        m_attachedMarkers = new Dictionary<Archetype, VisMarker>();

        m_boundsContainer = new GameObject("[MonoDirector] - Bounding Boxes");
        m_markerContainer = new GameObject("[MonoDirector] - Markers");
        
        for (int i = 0; i < MaxBoundingBoxes; i++)
        {
            GameObject frameObject = Object.Instantiate(BundleLoader.FrameObject, m_boundsContainer.transform);
            VisBounds boundingBox = new VisBounds(frameObject);
            boundingBox.Hide();
            m_bounds.Add(boundingBox);
        }
        
        for (int i = 0; i < MaxMarkers; i++)
        {
            GameObject markerObject = Object.Instantiate(BundleLoader.PropMarkerObject, m_markerContainer.transform);
            VisMarker marker = new VisMarker(markerObject);
            marker.Hide();
            m_markers.Add(marker);
        }

        Caster.OnActorRemoved += OnActorRemoved;
        Caster.OnActorSelected += OnActorSelected;
        Caster.OnActorDeselected += OnActorDeselected;

        Caster.OnPropAdded += OnPropAdded;
        Caster.OnPropRemoved += OnPropRemoved;
        Caster.OnPropSelected += OnPropSelected;
        Caster.OnPropDeselected += OnPropDeselected;
    }

    public static void Shutdown()
    {
        m_bounds.Clear();
        m_markers.Clear();
        
        Caster.OnActorRemoved -= OnActorRemoved;
        Caster.OnActorSelected -= OnActorSelected;
        Caster.OnActorDeselected -= OnActorDeselected;
        
        Caster.OnPropAdded -= OnPropAdded;
        Caster.OnPropRemoved -= OnPropRemoved;
        Caster.OnPropSelected -= OnPropSelected;
        Caster.OnPropDeselected -= OnPropDeselected;
    }

    public static void Update()
    {
        if (m_bounds == null || m_markers == null)
            return;
        
        foreach (var box in m_bounds)
            box.Update();
        
        foreach (var marker in m_markers)
            marker.Update();
    }

    public static void ShowBounds()
    {
        foreach (var box in m_bounds)
        {
            if (box.Attached)
                box.Show();
        }
    }

    public static void HideBounds()
    {
        foreach (var box in m_bounds)
        {
            if (box.Attached)
                box.Hide();
        }
    }

    public static void ShowMarkers()
    {
        foreach (var marker in m_markers)
        {
            if (marker.Attached)
                marker.Show();
        }
    }

    public static void HideMarkers()
    {
        foreach (var marker in m_markers)
        {
            if (marker.Attached)
                marker.Hide();
        }
    }
    
    public static void ShowAll()
    {
        ShowBounds();
        ShowMarkers();
    }

    public static void HideAll()
    {
        HideBounds();
        HideMarkers();
    }

    private static void AttachBounds(Actor actor)
    {
        if (m_attachedBounds.ContainsKey(actor))
            return;
        
        var bounds = m_bounds.First((bounds) => !bounds.Attached);
        
        Transform hips = actor.Avatar.animator.GetBoneTransform(HumanBodyBones.Hips);

        bounds.SetArchetype(actor);
        bounds.Attach(hips);
        bounds.SetSize(new Vector3(0.5f, 1f, 0.5f));
        bounds.Show();
        
        m_attachedBounds.Add(actor, bounds);
    }

    private static void AttachBounds(Prop prop)
    {
        if (m_attachedBounds.ContainsKey(prop))
            return;
        
        var bounds = m_bounds.First((bounds) => !bounds.Attached);

        MarrowBody body = prop.Entity.Bodies.First();

        bounds.SetArchetype(prop);
        bounds.Attach(prop.Entity.transform);
        bounds.SetOffset(body.Bounds.center);
        bounds.SetSize(body.Bounds.extents);
        bounds.Show();
        
        m_attachedBounds.Add(prop, bounds);
    }

    private static void DetachBounds(Actor actor)
    {
        if (!m_attachedBounds.TryGetValue(actor, out var bounds))
            return;
        
        bounds.Detach();
        bounds.Hide();
        
        m_attachedBounds.Remove(actor);
    }

    private static void DetachBounds(Prop prop)
    {
        if (!m_attachedBounds.TryGetValue(prop, out var bounds))
            return;
        
        bounds.Detach();
        bounds.Hide();
        
        m_attachedBounds.Remove(prop);
    }
    
    private static void AttachMarker(Actor actor)
    {
        if (m_attachedMarkers.ContainsKey(actor))
            return;
        
        var marker = m_markers.First((marker) => !marker.Attached);
        
        Transform head = actor.Avatar.animator.GetBoneTransform(HumanBodyBones.Head);
        
        marker.SetArchetype(actor);
        marker.Attach(head);
        marker.SetOffset(Vector3.up * 0.3f);
        marker.Show();
        
        m_attachedMarkers.Add(actor, marker);
    }

    private static void AttachMarker(Prop prop)
    {
        if (m_attachedMarkers.ContainsKey(prop))
            return;
        
        var marker = m_markers.First((marker) => !marker.Attached);

        marker.SetArchetype(prop);
        marker.Attach(prop.Entity.transform);
        marker.SetOffset(Vector3.up * 0.3f);
        marker.Show();
        
        m_attachedMarkers.Add(prop, marker);
    }

    private static void DetachMarker(Actor actor)
    {
        if (!m_attachedMarkers.TryGetValue(actor, out var marker))
            return;
        
        marker.Detach();
        marker.Hide();
        
        m_attachedMarkers.Remove(actor);
    }

    private static void DetachMarker(Prop prop)
    {
        if (!m_attachedMarkers.TryGetValue(prop, out var marker))
            return;

        marker.Detach();
        marker.Hide();
        
        m_attachedMarkers.Remove(prop);
    }

    private static void OnActorRemoved(Actor actor)
    {
        DetachBounds(actor);
        DetachMarker(actor);
    }
    
    private static void OnActorSelected(Actor actor)
    {
        AttachBounds(actor);
        AttachMarker(actor);
    }

    private static void OnActorDeselected(Actor actor)
    {
        DetachBounds(actor);
        DetachMarker(actor);
    }

    private static void OnPropAdded(Prop prop)
    {
        AttachBounds(prop);
        AttachMarker(prop);
    }

    private static void OnPropRemoved(Prop prop)
    {
        DetachBounds(prop);
        DetachMarker(prop);
    }
    
    private static void OnPropSelected(Prop prop)
    {
        AttachBounds(prop);
        AttachMarker(prop);
    }

    private static void OnPropDeselected(Prop prop)
    {
        DetachBounds(prop);
        DetachMarker(prop);
    }
}