using NEP.MonoDirector.Data;
using SLZ.Vehicle;
using System;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class TrackedVehicle : Prop
    {
        public TrackedVehicle(IntPtr ptr) : base(ptr) { }

        public Atv Vehicle { get => vehicle; }

        protected Atv vehicle;

        public void SetVehicle(Atv vehicle)
        {
            this.vehicle = vehicle;
        }

        public void Act()
        {

        }

        public override void Record(int frame)
        {
            isRecording = true;
        }
    }
}
