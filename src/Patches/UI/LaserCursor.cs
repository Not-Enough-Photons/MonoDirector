using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NEP.MonoDirector.Patches
{
    internal static class LaserCursor
    {
        [HarmonyLib.HarmonyPatch(typeof(SLZ.UI.LaserCursor), nameof(SLZ.UI.LaserCursor.OnPointerEnter))]
        internal static class OnPointerEnter
        {
            internal static void Postfix(PointerEventData eventData)
            {
                Main.Logger.Msg(eventData.pointerCurrentRaycast.worldPosition);
            }
        }
    }
}
