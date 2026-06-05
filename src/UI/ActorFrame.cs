using Il2CppSLZ.Marrow;
using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.Archetypes.Proxy;
using UnityEngine;

namespace NEP.MonoDirector.UI;

public class ActorFrame
{
    public ActorFrame(GameObject gameObject)
    {
        m_gameObject = gameObject;
    }

    public bool Active => m_gameObject != null && m_gameObject.activeInHierarchy;
    public bool HasTarget => m_target != null;

    private GameObject m_gameObject;
    private MeshRenderer m_renderer;
    private Transform m_target;
    private Vector3 m_offset;
    private Vector3 m_size = Vector3.one;

    public void SetTarget(Transform target)
    {
        m_target = target;
    }

    public void SetSize(Vector3 size)
    {
        m_size = size;
        m_gameObject.transform.localScale = m_size;
    }

    public void SetOffset(Vector3 offset)
    {
        m_offset = offset;
    }

    public void SetFrame(GameObject frame)
    {
        m_gameObject = frame;
    }

    public void Parent(ActorProxy actor)
    {
        if (actor == null)
            return;

        m_target = actor.Collider.transform;
        m_gameObject.transform.position = m_target.position;
    }

    public void Update()
    {
        if (m_gameObject == null)
            return;

        if (m_target == null)
            return;

        m_gameObject.transform.position = Vector3.Lerp(m_gameObject.transform.position, m_target.position + m_offset, 8f * Time.deltaTime);
        m_gameObject.transform.rotation = Quaternion.identity;
    }

    public void Show()
    {
        m_gameObject.SetActive(true);
    }

    public void Hide()
    {
        m_gameObject.SetActive(false);
    }
}
