using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Archetypes.Proxy;
using NEP.MonoDirector.State;
using NEP.MonoDirector.UI;
using UnityEngine;
using static Il2CppSLZ.Marrow.PuppetMasta.Muscle;

namespace NEP.MonoDirector.Archetypes;

public static class PropBuilder
{
    public static void BuildProp(MarrowEntity entity)
    {
        if (!entity)
            return;

        if (entity.Bodies.Count == 0)
            return;

        if (entity.GetComponent<PropProxy>())
            return;

        if (Prop.EligibleWithType<Gun>(entity))
        {
            BuildGunProp(entity);
            return;
        }

        if (Prop.EligibleWithType<ObjectDestructible>(entity))
        {
            BuildDestructibleProp(entity);
            return;
        }

        if (Prop.EligibleWithType<Magazine>(entity))
        {
            BuildMagazineProp(entity);
            return;
        }

        if (Prop.EligibleWithType<Atv>(entity))
        {
            BuildVehicleProp(entity);
            return;
        }

        if (Prop.IsActorProp(entity))
        {
#if DEBUG
            Logging.Msg($"Adding prop component to {entity.name}");
#endif
            Prop prop = new Prop();
            prop.CreateProxy(entity);

            Caster.AddRecordProp(prop);
        }
    }
    
    public static void RemoveProp(MarrowEntity entity)
    {
        // Prop already removed
        if (!entity.TryGetComponent(out PropProxy proxy))
            return;

        if (Director.PlayState != PlayState.Stopped)
            return;
        
#if DEBUG
        MelonLoader.MelonLogger.Msg($"Removing component from {entity.name}");
#endif
        /*if (prop is TrackedVehicle vehicle)
        {
            vehicle.RemoveVehicle();
        }*/

        Prop prop = proxy.Prop;
        
        Caster.RemoveProp(prop);
        Director.ActiveScene.RemoveProp(prop);
        prop.Delete();
    }

    private static void BuildGunProp(MarrowEntity entity)
    {
#if DEBUG
        Logging.Msg($"Adding gun component to {entity.name}");
#endif
        GunProp prop = new GunProp();

        prop.CreateProxy(entity);
        prop.SetGun(entity.GetComponent<Gun>());

        // Check for a loaded magazine
        if (prop.Gun.HasMagazine())
        {
            AmmoSocket ammoSocket = prop.Gun.ammoSocket;

            // Some guns do not have a magazine, but can still be loaded.
            // Shotguns do this for instance, so check if that's the case.
            if (ammoSocket.HasMagazine)
            {
                MarrowEntity magazine = ammoSocket._magazinePlug.magazine.interactableHost.marrowEntity;
                BuildMagazineProp(magazine);
            }
        }

        Caster.AddRecordProp(prop);
    }

    private static void BuildDestructibleProp(MarrowEntity entity)
    {
#if DEBUG
        Logging.Msg($"Adding destructable component to {entity.name}");
#endif
        BreakableProp prop = new BreakableProp();
        prop.CreateProxy(entity);
        prop.SetBreakableObject(entity.GetComponent<ObjectDestructible>());

        Caster.AddRecordProp(prop);
    }

    private static void BuildMagazineProp(MarrowEntity entity)
    {
#if DEBUG
        Logging.Msg($"Adding magazine component to {entity.name}");
#endif
        Prop prop = new Prop();
        prop.CreateProxy(entity);

        Caster.AddRecordProp(prop);
    }

    private static void BuildVehicleProp(MarrowEntity entity)
    {
#if DEBUG
        Logging.Msg($"Adding vehicle component to {entity.name}");
#endif
        Prop prop = new Prop();
        prop.CreateProxy(entity);
        //prop.SetVehicle(entity.GetComponent<Atv>());

        Caster.AddRecordProp(prop);
    }
}
