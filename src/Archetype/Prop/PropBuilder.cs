using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Proxy;
using NEP.MonoDirector.UI;
using UnityEngine;
using static Il2CppSLZ.Marrow.PuppetMasta.Muscle;

namespace NEP.MonoDirector.Archetype;

public static class PropBuilder
{
    public static void BuildProp(MarrowEntity entity)
    {
        if (!entity)
            return;

        if (entity.Bodies.Count == 0)
            return;

        if (entity.GetComponent<Prop>())
            return;

        if (Prop.EligibleWithType<Gun>(entity))
        {
#if DEBUG
            Logging.Msg($"Adding gun component to {entity.name}");
#endif
            GunProp prop = entity.gameObject.AddComponent<GunProp>();

            prop.SetEntity(entity);
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
                    BuildProp(magazine);
                }
            }

            Caster.AddRecordProp(prop);
            PropFrameManager.AddFrameToProp(prop);
            return;
        }

        if (Prop.EligibleWithType<ObjectDestructible>(entity))
        {
#if DEBUG
            Logging.Msg($"Adding destructable component to {entity.name}");
#endif
            BreakableProp prop = entity.gameObject.AddComponent<BreakableProp>();
            prop.SetEntity(entity);
            prop.SetBreakableObject(entity.GetComponent<ObjectDestructible>());

            Caster.AddRecordProp(prop);
            PropFrameManager.AddFrameToProp(prop);
            return;
        }

        if (Prop.EligibleWithType<Magazine>(entity))
        {
#if DEBUG
            Logging.Msg($"Adding magazine component to {entity.name}");
#endif
            Prop prop = entity.gameObject.AddComponent<Prop>();
            prop.SetEntity(entity);

            Caster.AddRecordProp(prop);
            PropFrameManager.AddFrameToProp(prop);
            return;
        }

        if (Prop.EligibleWithType<Atv>(entity))
        {
#if DEBUG
            Logging.Msg($"Adding vehicle component to {entity.name}");
#endif
            Prop prop = entity.gameObject.AddComponent<Prop>();
            prop.SetEntity(entity);
            //prop.SetVehicle(entity.GetComponent<Atv>());

            Caster.AddRecordProp(prop);
            PropFrameManager.AddFrameToProp(prop);
            return;
        }

        if (Prop.IsActorProp(entity))
        {
#if DEBUG
            Logging.Msg($"Adding prop component to {entity.name}");
#endif
            Prop prop = entity.gameObject.AddComponent<Prop>();
            prop.SetEntity(entity);

            Caster.AddRecordProp(prop);
            PropFrameManager.AddFrameToProp(prop);
        }
    }
    
    public static void RemoveProp(MarrowEntity entity)
    {
        // Prop already removed
        if (!entity.TryGetComponent(out Prop prop))
            return;

        if (Director.PlayState == State.PlayState.Stopped)
        {
#if DEBUG
            MelonLoader.MelonLogger.Msg($"Removing component from {entity.name}");
#endif
            if (prop is TrackedVehicle vehicle)
            {
                vehicle.RemoveVehicle();
            }

            Caster.RemoveProp(prop);
            PropFrameManager.RemoveFrameFromProp(prop);
            MarkerManager.RemoveMarkerFromProp(prop);
            Director.ActiveScene.RemoveProp(prop);
            GameObject.Destroy(prop);
        }
    } 
}
