using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NEP.MonoDirector.Patches
{
    internal static class VRGraphicRaycaster
    {
        [HarmonyLib.HarmonyPatch(typeof(SLZ.UI.LaserCursor), nameof(SLZ.UI.LaserCursor.Trigger))]
        internal static class Trigger
        {
            internal static void Postfix(SLZ.UI.LaserCursor __instance)
            {
                var module = __instance.module;
                var eventSystem = module.GetBaseEventData().m_EventSystem;
                
                if (eventSystem.currentSelectedGameObject != null)
                {
                    Main.Logger.Msg(eventSystem.currentSelectedGameObject.name);
                }
            }
        }
    }
}
