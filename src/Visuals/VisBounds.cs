using Il2CppSLZ.Marrow;
using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Content;
using UnityEngine;

namespace NEP.MonoDirector.Visuals;

public sealed class VisBounds
{
    public VisBounds(GameObject prefab)
    {
        m_object = prefab;
    }
    
    public Archetype Archetype => m_archetype;
    public bool Attached => m_attached;

    private Archetype m_archetype;
    private bool m_attached;

    private GameObject m_object;
    private Transform m_target;
    private Vector3 m_offset;
    private Vector3 m_size;
    
    public void Attach(Transform target)
    {
        m_target = target;
    }

    public void Detach()
    {
        m_archetype = null;
        m_target = null;
    }

    public void Update()
    {
        if (!m_object || !m_target)
            return;
        
        m_object.transform.position = m_target.position + m_offset;
        m_object.transform.rotation = Quaternion.identity;
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

    public void SetSize(Vector3 size)
    {
        m_size = size;
        m_object.transform.localScale = m_size;
    }
    
    public void SetArchetype(Archetype archetype)
    {
        m_archetype = archetype;
        m_attached = true;
    }
}