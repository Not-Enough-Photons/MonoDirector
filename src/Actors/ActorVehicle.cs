using NEP.MonoDirector.Data;
using SLZ.Vehicle;
using System;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorVehicle : ActorProp
    {
        public ActorVehicle(IntPtr ptr) : base(ptr) { }

        public Atv Vehicle { get => vehicle; }

        protected Atv vehicle;

        public void SetVehicle(Atv vehicle)
        {
            this.vehicle = vehicle;
        }

        public override void Act(int currentTick)
        {
            if (!propFrames.ContainsKey(currentTick))
            {
                return;
            }

            var propFrame = propFrames[currentTick];

            gameObject.SetActive(true);

            if (vehicle == null)
            {
                return;
            }

            InteractableRigidbody.isKinematic = true;

            vehicle.mainBody.position = propFrame.position;
            vehicle.mainBody.rotation = propFrame.rotation;
        }

        public override void Record(int frame)
        {
            isRecording = true;

            if (!propFrames.ContainsKey(frame))
            {
                if (InteractableRigidbody != null && !InteractableRigidbody.IsSleeping())
                {
                    propFrames.Add(frame, new ObjectFrame(transform));
                }
            }
        }
    }
}
