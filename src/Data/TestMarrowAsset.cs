using SLZ.Marrow.Warehouse;
using UnityEngine;

namespace NEP.MonoDirector.Data
{
    public class TestMarrowAsset : MarrowGameObject
    {
        public TestMarrowAsset(GameObject original)
        {
            Asset = original;
        }

        public GameObject Asset { get; }
    }
}