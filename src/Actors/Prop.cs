using System.Collections.Generic;

using NEP.MonoDirector.Data;

using SLZ.Interaction;
using SLZ.Props;
using SLZ.Props.Weapons;

using UnityEngine;

using Il2CppSystem;

using SLZ.Vehicle;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class Prop : MonoBehaviour
    {
        public Prop(System.IntPtr ptr) : base(ptr) { }

        public Trackable Actor { get => actor; }

        public List<ObjectFrame> PropFrames { get => propFrames; }
        public Rigidbody InteractableRigidbody { get => interactableRigidbody; }
        public bool isRecording;

        public static readonly Type[] whitelistedTypes = new Type[]
        {
            UnhollowerRuntimeLib.Il2CppType.Of<Gun>(),
            UnhollowerRuntimeLib.Il2CppType.Of<Magazine>(),
            UnhollowerRuntimeLib.Il2CppType.Of<ObjectDestructable>(),
            UnhollowerRuntimeLib.Il2CppType.Of<Atv>()
        };

        private Trackable actor;
        private Rigidbody interactableRigidbody;

        protected int stateTick;
        protected int recordedTicks;

        private ObjectFrame previousFrame;
        private ObjectFrame nextFrame;

        protected List<ObjectFrame> propFrames;

        protected virtual void Awake()
        {
            propFrames = new List<ObjectFrame>();
        }

        public static bool IsActorProp(Rigidbody rigidbody)
        {
            if (rigidbody == null)
            {
                return false;
            }

            if (rigidbody.isKinematic || rigidbody.gameObject.isStatic)
            {
                return false;
            }

            if (rigidbody.gameObject.layer == LayerMask.NameToLayer("EnemyColliders"))
            {
                return false;
            }

            if(rigidbody.GetComponent<InteractableHost>() == null)
            {
                return false;
            }

            if (rigidbody.GetComponent<Prop>() != null || rigidbody.GetComponent<WorldGrip>() != null)
            {
                return false;
            }

            return true;
        }

        public static bool EligibleWithType<T>(Rigidbody rigidbody)
        {
            return rigidbody.GetComponent<T>() != null;
        }

        public void SetRigidbody(Rigidbody rigidbody)
        {
            interactableRigidbody = rigidbody;
        }

        public void SetActor(Trackable actor)
        {
            this.actor = actor;
        }

        public void SetPhysicsActive(bool enable)
        {
            interactableRigidbody.isKinematic = enable;
        }

        public virtual void Act()
        {
            gameObject.SetActive(true);

            if(interactableRigidbody == null)
            {
                interactableRigidbody = GetComponent<Rigidbody>();
            }
            else
            {
                interactableRigidbody.isKinematic = true;
            }

            previousFrame = new ObjectFrame();
            nextFrame = new ObjectFrame();

            foreach (var frame in propFrames)
            {
                previousFrame = nextFrame;
                nextFrame = frame;

                if (frame.frameTime > Playback.instance.PlaybackTime)
                {
                    break;
                }
            }

            float gap = nextFrame.frameTime - previousFrame.frameTime;
            float head = Playback.instance.PlaybackTime - previousFrame.frameTime;

            float delta = head / gap;

            transform.position = Vector3.Lerp(previousFrame.position, nextFrame.position, delta);
            transform.rotation = Quaternion.Slerp(previousFrame.rotation, nextFrame.rotation, delta);
        }

        public virtual void Record(int frame)
        {
            isRecording = true;

            ObjectFrame objectFrame = new ObjectFrame()
            {
                transform = transform,
                position = transform.position,
                rotation = transform.rotation,
                scale = transform.localScale,
                frameTime = Recorder.instance.RecordingTime
            };

            if (frame == 0 || interactableRigidbody != null && interactableRigidbody.IsSleeping())
            {
                propFrames.Add(objectFrame);
            }
            else
            {
                propFrames.Add(objectFrame);
                recordedTicks++;
            }
        }

        public void ResetTicks()
        {
            stateTick = 0;
        }
    }
}
