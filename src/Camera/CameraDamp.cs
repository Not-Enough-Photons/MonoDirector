using NEP.MonoDirector.State;
using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class CameraDamp : MonoBehaviour
    {
        public CameraDamp(System.IntPtr ptr) : base(ptr) { }

        public float delta = 4f;

        private Quaternion lastRotation;

        private void LateUpdate()
        {
            //lastRotation = transform.rotation;
            //transform.rotation = Quaternion.Slerp(lastRotation, transform.rotation, delta * Time.deltaTime);
        }
    }
}
