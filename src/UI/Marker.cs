using Il2CppSLZ.Marrow;
using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.Archetypes.Proxy;
using NEP.MonoDirector.Tools;
using UnityEngine;

namespace NEP.MonoDirector.UI;

public class Marker
{
    public Marker(GameObject gameObject)
    {
        m_gameObject = gameObject;
        m_markerType = MarkerType.None;

        m_renderer = m_gameObject.GetComponent<MeshRenderer>();
    }

    public enum MarkerType
    {
        None,
        Prop,
        Actor
    }

    public enum PropType
    {
        None,
        Generic,
        Gun,
        Magazine,
        Vehicle,
        Tool,
        Camera
    }

    public enum ActorType
    {
        None,
        Shown,
        Hidden
    }

    public bool Active => m_gameObject.activeInHierarchy;
    public bool HasTarget => m_target != null;

    private GameObject m_gameObject;
    private Texture2D m_iconTexture;
    private MeshRenderer m_renderer;
    private Transform m_target;
    private Vector3 m_offset;

    private MarkerType m_markerType = MarkerType.None;
    private PropType m_propType = PropType.None;
    private ActorType m_actorType = ActorType.None;

    private const float m_markerPadding = 0.25f;

    public void SetTarget(Transform target)
    {
        m_target = target;
    }

    public void SetOffset(Vector3 offset)
    {
        m_offset = offset;
    }

    public void SetMarker(GameObject marker)
    {
        m_gameObject = marker;
    }

    public void Parent(Prop prop)
    {
        Reset();

        if (prop == null)
            return;

        m_markerType = MarkerType.Prop;

        if (Prop.EligibleWithType<Gun>(prop.Proxy.Entity))
            m_propType = PropType.Gun;
        else if (Prop.EligibleWithType<Magazine>(prop.Proxy.Entity))
            m_propType = PropType.Magazine;
        else if (Prop.EligibleWithType<Atv>(prop.Proxy.Entity))
            m_propType = PropType.Vehicle;
        else if (Prop.EligibleWithType<PointToolEntity>(prop.Proxy.Entity))
            m_propType = PropType.Tool;
        else if (Prop.EligibleWithType<HandheldCamera>(prop.Proxy.Entity))
            m_propType = PropType.Camera;
        else
            m_propType = PropType.Generic;

        UpdateIcon();

        m_target = prop.Proxy.transform;
        m_gameObject.transform.position = prop.Proxy.transform.position;

        if (!prop.Proxy)
        {
            Hide();
            return;
        }

        Bounds bounds = prop.Proxy.Entity.AnchorBody.Bounds;
        SetOffset(new Vector3(0f, (bounds.center.y + bounds.extents.y) + m_markerPadding, 0f));
    }

    public void Parent(ActorProxy actor)
    {
        Reset();

        if (actor == null)
            return;

        m_markerType = MarkerType.Actor;

        if (actor.Actor.Hidden)
            m_actorType = ActorType.Hidden;
        else
            m_actorType = ActorType.Shown;

        UpdateIcon();

        m_target = actor.Actor.ActorBody.Head.transform;
        m_gameObject.transform.position = m_target.position;
        SetOffset(Vector3.up * 0.3f);
    }

    public void Update()
    {
        if (m_gameObject == null)
            return;

        if (m_target == null)
            return;

        m_gameObject.transform.position = Vector3.Lerp(m_gameObject.transform.position, m_target.position + m_offset, 8f * Time.deltaTime);
    }

    public void Show()
    {
        m_gameObject.SetActive(true);
    }

    public void Hide()
    {
        m_gameObject.SetActive(false);
    }

    public void OnActorUpdated(Actor actor)
    {
        if (actor.Hidden)
            m_actorType = ActorType.Hidden;
        else
            m_actorType = ActorType.Shown;

        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (m_markerType == MarkerType.None) 
            return;

        if (m_markerType == MarkerType.Prop)
        {
            switch (m_propType)
            {
                case PropType.None:
                    m_iconTexture = null;
                    break;
                case PropType.Gun:
                    m_iconTexture = BundleLoader.IconGun;
                    break;
                case PropType.Magazine:
                    m_iconTexture = BundleLoader.IconMagazine;
                    break;
                case PropType.Vehicle:
                    m_iconTexture = BundleLoader.IconVehicle;
                    break;
                case PropType.Tool: 
                case PropType.Camera:
                case PropType.Generic:
                default:
                    m_iconTexture = BundleLoader.IconProp;
                    break;
            }
        }

        if (m_markerType == MarkerType.Actor)
        {
            switch (m_actorType)
            {
                case ActorType.Hidden:
                    m_iconTexture = BundleLoader.IconHidden;
                    break;
                case ActorType.Shown:
                    m_iconTexture = BundleLoader.IconVisible;
                    break;
                case ActorType.None:
                    m_iconTexture = null;
                    break;
            }
        }

        m_renderer.material.SetTexture("_Texture2D", m_iconTexture);
    }

    private void Reset()
    {
        m_markerType = MarkerType.None;
        m_actorType = ActorType.None;
        m_propType = PropType.None;
    }
}
