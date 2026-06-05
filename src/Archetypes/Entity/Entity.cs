using NEP.MonoDirector.Tools;
using UnityEngine;

namespace NEP.MonoDirector.Archetypes;

public abstract class Entity : Archetype
{
    public enum Type
    {
        None,
        Light,
        Sound,
        Unknown
    }

    public virtual Type EntityType => Type.None;

    public Vector3 Position => m_position;
    public Quaternion Rotation => m_rotation;
    public string Barcode => m_barcode;
    public PointToolEntity AssociatedTool => m_associatedTool;
    
    protected Vector3 m_position;
    protected Quaternion m_rotation;
    protected string m_barcode;
    protected PointToolEntity m_associatedTool;

    public void SetPosition(Vector3 position)
    {
        m_position = position;
    }

    public void SetRotation(Quaternion rotation)
    {
        m_rotation = rotation;
    }

    public void SetBarcode(string barcode)
    {
        m_barcode = barcode;
    }

    public void AssociateTool(PointToolEntity tool)
    {
        m_associatedTool = tool;
    }
}