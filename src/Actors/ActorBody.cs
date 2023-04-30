using System.Collections.Generic;

using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using SLZ.Interaction;
using SLZ.Rig;
using SLZ.Vehicle;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    public class ActorBody
    {
        public ActorBody(Actor actor, PhysicsRig physicsRig)
        {
            this.actor = actor;
            this.physicsRig = physicsRig;
            this.physTorso = physicsRig.torso;

            rigidbodies = new List<Rigidbody>();
            limbFrames = new List<FrameGroup>();
            clonedLimbFrames = new List<Transform>();

            rigidbodies.Add(physTorso.rbHead);
            rigidbodies.Add(physTorso.rbSpine);
            rigidbodies.Add(physTorso.rbChest);
            rigidbodies.Add(physTorso.rbPelvis);
        }

        private Actor actor;
        private PhysicsRig physicsRig;
        private PhysTorso physTorso;

        private List<Rigidbody> rigidbodies;

        private List<FrameGroup> limbFrames;
        private List<Transform> clonedLimbFrames;

        private FrameGroup previousFrame;
        private FrameGroup nextFrame;

        public void Act()
        {
            previousFrame = new FrameGroup();
            nextFrame = new FrameGroup();

            foreach (var frame in limbFrames)
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

            List<ObjectFrame> previousTransformFrames = previousFrame.transformFrames;
            List<ObjectFrame> nextTransformFrames = nextFrame.transformFrames;

            for (int i = 0; i < 55; i++)
            {
                if (i == (int)HumanBodyBones.Jaw)
                {
                    continue;
                }

                var bone = clonedLimbFrames[i];

                if (bone == null)
                {
                    continue;
                }

                if (previousTransformFrames == null)
                {
                    continue;
                }

                Vector3 previousBonePosition = previousTransformFrames[i].position;
                Vector3 nextBonePosition = nextTransformFrames[i].position;

                Quaternion previousBoneRotation = previousTransformFrames[i].rotation;
                Quaternion nextBoneRotation = nextTransformFrames[i].rotation;

                bone.position = Vector3.Lerp(previousBonePosition, nextBonePosition, delta);
                bone.rotation = Quaternion.Slerp(previousBoneRotation, nextBoneRotation, delta);
            }
        }

        public void RecordFrame()
        {
            FrameGroup frameGroup = new FrameGroup();
            frameGroup.SetFrames(CaptureLimbs(rigidbodies.ToArray()), Recorder.instance.RecordingTime);
            limbFrames.Add(frameGroup);
        }

        public void Clone()
        {
            foreach (Rigidbody rb in rigidbodies)
            {
                GameObject cloneRb = GameObject.Instantiate(rb.gameObject);
                cloneRb.GetComponent<Rigidbody>().isKinematic = true;
                clonedLimbFrames.Add(cloneRb.transform);
            }

            SanitizePhysicsTorso(clonedLimbFrames);
        }

        private void SanitizePhysicsTorso(List<Transform> clonedLimbBones)
        {
            // let's start with sanitizing the head
            Il2CppSystem.Type[] whitelist = new Il2CppSystem.Type[]
            {
                UnhollowerRuntimeLib.Il2CppType.Of<Rigidbody>(),
                UnhollowerRuntimeLib.Il2CppType.Of<MeshCollider>()
            };

            for(int i = 0; i < clonedLimbBones.Count; i++)
            {
                if(i == 0)
                {
                    Sanitize(whitelist, clonedLimbBones[i], "col_skull");
                }
                else
                {
                    Sanitize(whitelist, clonedLimbBones[i]);
                }
            }
        }
        
        private void Sanitize(Il2CppSystem.Type[] whitelist, Transform transform, string objectToKeep = "")
        {
            MonoBehaviour[] components = transform.GetComponentsInChildren<MonoBehaviour>();

            // Reverse loop should delete required components
            for(int i = 0; i < components.Length; i++)
            {
                for(int j = 0; j < whitelist.Length; j++)
                {
                    MonoBehaviour component = components[i];
                    Il2CppSystem.Type whitelistedComponent = whitelist[j];

                    var componentType = component.GetIl2CppType();
                    var whitelistedType = whitelistedComponent.GetIl2CppType();

                    if(componentType != whitelistedType)
                    {
                        GameObject.Destroy(components[i]);
                    }
                }
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                if (child.name != objectToKeep)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }

        private List<ObjectFrame> CaptureLimbs(Rigidbody[] rigidbodies)
        {
            List<ObjectFrame> frames = new List<ObjectFrame>();

            for (int i = 0; i < rigidbodies.Length; i++)
            {
                ObjectFrame frame = new ObjectFrame(rigidbodies[i]);
                //frame.currentTime = Recorder.instance.RecordingTime;
                frames.Add(frame);
            }

            return frames;
        }
    }
}
