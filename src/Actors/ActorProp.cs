using System.Collections.Generic;
using System.Linq;

using NEP.MonoDirector.Data;

using SLZ.Interaction;
using SLZ.Props;
using SLZ.Props.Weapons;

using UnityEngine;

using Il2CppSystem;

using UnhollowerRuntimeLib;
using Jevil;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorProp : MonoBehaviour
    {
        public ActorProp(System.IntPtr ptr) : base(ptr) { }

        public Actor Actor { get => actor; }

        public Dictionary<int, ObjectFrame> PropFrames { get => propFrames; }
        public Rigidbody InteractableRigidbody { get => interactableRigidbody; }
        public bool isRecording;

        public static readonly Type[] whitelistedTypes = new Type[]
        {
            UnhollowerRuntimeLib.Il2CppType.Of<Gun>(),
            UnhollowerRuntimeLib.Il2CppType.Of<Magazine>(),
            UnhollowerRuntimeLib.Il2CppType.Of<ObjectDestructable>()
        };

        private Actor actor;
        private Rigidbody interactableRigidbody;

        protected int stateTick;
        protected int recordedTicks;

        protected Dictionary<int, ObjectFrame> propFrames;

        protected virtual void Awake()
        {
            propFrames = new Dictionary<int, ObjectFrame>();
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

            if (rigidbody.GetComponent<ActorProp>() != null || rigidbody.GetComponent<WorldGrip>() != null)
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

        public void SetActor(Actor actor)
        {
            this.actor = actor;
        }

        public void SetPhysicsActive(bool enable)
        {
            interactableRigidbody.isKinematic = enable;
        }

        public virtual void Act(int currentTick)
        {
            if (!propFrames.ContainsKey(currentTick))
            {
                return;
            }

            var propFrame = propFrames[currentTick];

            gameObject.SetActive(true);

            if(interactableRigidbody == null)
            {
                interactableRigidbody = GetComponent<Rigidbody>();
            }
            else
            {
                interactableRigidbody.isKinematic = true;
            }

            if (propFrame.transform == null)
            {
                return;
            }

            propFrame.transform.position = propFrame.position;
            propFrame.transform.rotation = propFrame.rotation;
        }

        public virtual void Record(int frame)
        {
            isRecording = true;

            if (!propFrames.ContainsKey(frame))
            {
                if(frame == 0 || interactableRigidbody != null && interactableRigidbody.IsSleeping())
                {
                    propFrames.Add(frame, new ObjectFrame(transform));
                }
                else
                {
                    propFrames.Add(frame, new ObjectFrame(transform));
                    recordedTicks++;
                }
            }
        }

        public void ResetTicks()
        {
            stateTick = 0;
        }
    }
}
