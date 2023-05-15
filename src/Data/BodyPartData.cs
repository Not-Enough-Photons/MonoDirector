using UnityEngine;

using SLZ.Rig;

namespace NEP.MonoDirector.Data
{
    public struct BodyPartData
    {
        public BodyPartData(Transform transform)
        {
            position = transform.position;
            rotation = transform.rotation;
        }

        public Vector3 position;
        public Quaternion rotation;
    }
}
