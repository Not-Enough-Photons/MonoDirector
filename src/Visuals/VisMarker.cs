using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Content;
using UnityEngine;
using Type = NEP.MonoDirector.Archetypes.Type;

namespace NEP.MonoDirector.Visuals;

public sealed class VisMarker
{
    public VisMarker(GameObject prefab)
    {
        m_object = prefab;
        m_renderer = m_object.GetComponent<MeshRenderer>();
    }
    
    public Archetype Archetype => m_archetype;
    public bool Attached => m_attached;

    private Archetype m_archetype;
    private bool m_attached;

    private GameObject m_object;
    private Transform m_target;
    private Vector3 m_offset;
    private MeshRenderer m_renderer;
    
    public void Attach(Transform target)
    {
        m_target = target;
        m_attached = true;
    }

    public void Detach()
    {
        m_archetype = null;
        m_target = null;
        m_attached = false;
    }

    public void Update()
    {
        if (!m_object || !m_target)
            return;
        
        m_object.transform.position = m_target.position + m_offset;
    }

    public void Show()
    {
        m_object.SetActive(true);
    }

    public void Hide()
    {
        m_object.SetActive(false);
    }

    public void SetOffset(Vector3 offset)
    {
        m_offset = offset;
    }
    
    public void SetArchetype(Archetype archetype)
    {
        m_archetype = archetype;
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (m_archetype.Type == Type.None)
            throw new NotImplementedException("None type case is not implemented.");

        if (m_archetype.Type == Type.Actor)
            UpdateActorIcon();
        else if (m_archetype.Type == Type.Prop)
            UpdatePropIcon();
    }

    private void UpdateActorIcon()
    {
        Actor actor = (Actor)m_archetype;
        
        if (actor.Visible)
            SetTexture(BundleLoader.IconVisible);
        else
            SetTexture(BundleLoader.IconHidden);
    }

    private void UpdatePropIcon()
    {
        Prop prop = (Prop)m_archetype;

        switch (prop.PropType)
        {
            case PropType.Gun:
                SetTexture(BundleLoader.IconGun);
                break;
            case PropType.Magazine:
                SetTexture(BundleLoader.IconMagazine);
                break;
            case PropType.Vehicle:
                SetTexture(BundleLoader.IconVehicle);
                break;
            case PropType.Generic:
            default:
                SetTexture(BundleLoader.IconProp);
                break;
        }
    }

    private void SetTexture(Texture2D texture)
    {
        m_renderer.material.SetTexture("_Texture2D", texture);
    }
}