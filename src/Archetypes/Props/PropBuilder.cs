using Il2CppSLZ.Marrow.Interaction;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Extensions;

namespace NEP.MonoDirector.Archetypes;

public static class PropBuilder
{
    public static Dictionary<MarrowEntity, Prop> EntityTable;

    public static void Initialize()
    {
        EntityTable = new Dictionary<MarrowEntity, Prop>();
    }
    
    public static void CreateProp(MarrowEntity entity)
    {
        if (!entity)
            return;

        if (entity.Bodies.Count == 0)
            return;

        if (EntityTable.ContainsKey(entity))
            return;

        Prop prop = new Prop(entity);
        prop.Arm();
        
        EntityTable.Add(entity, prop);
        
        Caster.AddProp(prop);
        Director.Scene.AddArchetype(prop);
    }

    public static void DestroyProp(MarrowEntity entity)
    {
        if (!EntityTable.TryGetValue(entity, out Prop prop))
            return;

        prop.Destroy();

        EntityTable.Remove(entity);
        
        Caster.RemoveProp(prop);
        Director.Scene.RemoveArchetype(prop);
    }
}