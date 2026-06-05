using UnityEngine;

using NEP.MonoDirector.Data;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Archetypes;

public class BreakableProp : Prop
{
    public ObjectDestructible breakableProp;

    public override void OnSceneBegin()
    {
        base.OnSceneBegin();

        foreach(var action in m_actions)
        {
            action.Reset();
        }
    }

    public void SetBreakableObject(ObjectDestructible destructable)
    {
        this.breakableProp = destructable;
    }

    public void DestructionEvent()
    {
        breakableProp._isDead = false;
        breakableProp.TakeDamage(Vector3.zero, 100f, true);
        // gameObject.SetActive(false);
    }
}
