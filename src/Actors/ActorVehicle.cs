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

            if (currentTick == 1)
            {
                vehicle.transform.position = propFrame.position;
                vehicle.transform.rotation = propFrame.rotation;
                vehicle.transform.localScale = propFrame.scale;
            }

            vehicle.mainBody.velocity = propFrame.rigidbodyVelocity;
            vehicle.mainBody.angularVelocity = propFrame.rigidbodyAngularVelocity;
        }

        public override void Record(int frame)
        {
            isRecording = true;

            if (!propFrames.ContainsKey(frame))
            {
                if (InteractableRigidbody != null)
                {
                    if(frame == 1)
                    {
                        propFrames.Add(frame, new ObjectFrame(InteractableRigidbody));
                    }
                    else
                    {
                        propFrames.Add(frame, new ObjectFrame(InteractableRigidbody));
                    }
                }
            }
        }
    }
}
