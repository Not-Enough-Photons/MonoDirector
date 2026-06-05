using System.Text;

using NEP.MonoDirector.Archetypes.Proxy;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.State;
using NEP.MonoDirector.UI;
using NEP.MonoDirector.Yielding;

using UnityEngine;
using MelonLoader;

using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Warehouse;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;
using MarrowSeat = Il2CppSLZ.Marrow.Seat;

namespace NEP.MonoDirector.Archetypes;

public class Actor : Archetype, IBinaryData
{
    public enum ActionType : byte
    {
        ActorSwitchAvatar
    }
    
    public Actor()
    {
        m_ownedProps = new List<Prop>();
    }
    
    public Actor(MarrowAvatar avatar)
    {
        m_avatarCrate = Constants.RigManager.AvatarCrate.Crate;
        
        m_playerAvatar = avatar;

        GameObject micObject = new GameObject("[TEMP] - Actor Microphone");
        m_microphone = micObject.AddComponent<ActorSpeech>();

        m_ownedProps = new List<Prop>();
        
#if DEBUG
        GameObject proxyObject = new GameObject("[TEMP] - Actor Proxy");
        m_proxy = proxyObject.AddComponent<ActorProxy>();
        m_proxy.SetAnimator(m_playerAvatar.animator);
#endif
    }

    public Actor(AvatarCrate avatarCrate, MarrowAvatar avatar)
    {
        m_avatarCrate = avatarCrate;
        
        m_playerAvatar = avatar;

        GameObject micObject = new GameObject("[TEMP] - Actor Microphone");
        m_microphone = micObject.AddComponent<ActorSpeech>();

        m_ownedProps = new List<Prop>();
        
#if DEBUG
        GameObject proxyObject = new GameObject("[TEMP] - Actor Proxy");
        m_proxy = proxyObject.AddComponent<ActorProxy>();
        m_proxy.SetAnimator(m_playerAvatar.animator);
#endif
    }

    // For a traditional rig, this should be all the "head" bones
    public readonly List<int> HeadBones = new List<int>()
    {
        (int)HumanBodyBones.Head,
        (int)HumanBodyBones.Jaw,
        (int)HumanBodyBones.LeftEye,
        (int)HumanBodyBones.RightEye,
    };

    public string ActorName { get => m_actorName; }
    
    private AvatarCrate m_avatarCrate;
    public AvatarCrate AvatarCrate => m_avatarCrate;
    public MarrowAvatar Avatar { get => m_avatar; }

    public MarrowEntity MarrowEntity { get => m_marrowEntity; }

    public IReadOnlyList<FrameGroup> Frames => m_frames.AsReadOnly();
    public IReadOnlyList<Prop> OwnedProps => m_ownedProps.AsReadOnly();

    public ActorBody ActorBody { get => m_body; }
    public ActorProxy Proxy { get => m_proxy; }
    public ActorSpeech Microphone { get => m_microphone; }

    public bool Seated { get => m_activeSeat != null; }
    public bool Hidden { get => m_hidden; }

    private string m_actorName;
    
    private ActorBody m_body;
    private ActorProxy m_proxy;
    private ActorSpeech m_microphone;

    private List<Prop> m_ownedProps;

    private MarrowEntity m_marrowEntity;

    private MarrowSeat m_activeSeat;

    private MarrowAvatar m_playerAvatar;
    private MarrowAvatar m_avatar;

    private Transform[] m_avatarBones;
    private Transform[] m_clonedRigBones;

    private new FrameGroup m_previousFrame;
    private new FrameGroup m_nextFrame;

    private Transform m_lastPelvisParent;
    private int m_headIndex;
    private bool m_hidden;
    
    // Debug build stuff
    #if DEBUG
    private Transform[] m_previousFrameDebugger;
    private Transform[] m_nextFrameDebugger;
    #endif

    public override void OnSceneBegin()
    {
        foreach (var action in m_actions)
            action.Reset();
        
        // m_body.AllowCollisions(true);

        m_proxy.PoseAtStart();
    }

    public override void OnSceneEnd()
    {
        
    }

    public override void Perform()
    {
        m_previousFrame = new FrameGroup();
        m_nextFrame = new FrameGroup();

        for(int i = 0; i < m_frames.Count; i++)
        {
            var frame = m_frames[i];

            m_previousFrame = m_nextFrame;
            m_nextFrame = frame;

            if (frame.FrameTime > Playback.Instance.PlaybackTime)
            {
                break;
            }
        }

        // Nothing to show
        if (!m_proxy)
            return;
        
        m_proxy.Pose(m_previousFrame, m_nextFrame);
        
        for(int i = 0; i < m_actions.Count; i++)
        {
            var actionFrame = m_actions[i];

            if(Playback.Instance.PlaybackTime < actionFrame.timestamp)
            {
                continue;
            }
            else
            {
                actionFrame.Run();
            }
        }

        m_microphone?.Playback();
        m_microphone?.UpdateJaw();
    }

    /// <summary>
    /// Records the actor's bones, positons, and rotations for this frame.
    /// </summary>
    /// <param name="index">The frame to record the bones.</param>
    public override void Record()
    {
        m_frames.Add(m_proxy.Record());
    }

    public override void RecordAction(byte type, Action action)
    {
        if (Director.PlayState != PlayState.Recording)
            return;
        
        m_actions.Add(new ActionFrame(type, action, Recorder.Instance.RecordingTime));
    }

    public void CastActor()
    {
        CreateProxy(m_playerAvatar);
        UpdateClone();
    }

    public void SetAvatar(MarrowAvatar avatar)
    {
        m_playerAvatar = avatar;
        m_avatar = avatar;
    }

    public void CreateProxy(MarrowAvatar avatar)
    {
        if (!avatar)
            return;

        DestroyProxy();
        GameObject clone = GameObject.Instantiate(avatar.gameObject);
        m_proxy = clone.AddComponent<ActorProxy>();
        m_avatar = clone.GetComponent<MarrowAvatar>();
        m_body = new ActorBody(this, Constants.RigManager.physicsRig);
        m_body.AllowCollisions(false);
        m_marrowEntity = m_avatar.gameObject.AddComponent<MarrowEntity>();
        m_marrowEntity.Validate();
    }

    public void DestroyProxy()
    {
        if (!m_proxy)
            return;
        
        GameObject.Destroy(m_proxy);
        m_proxy = null;
    }
    
    public void UpdateClone()
    {
        // stops position overrides, if there are any
        Animator animator = m_avatar.GetComponent<Animator>() ?? m_avatar.animator;
        animator.enabled = false;
        m_proxy.SetAnimator(animator);
        
        GameObject.Destroy(m_avatar.GetComponent<LODGroup>());
        
        m_actorName = Constants.RigManager.AvatarCrate.Crate.Title;
        m_avatar.name = m_actorName;
        ShowHairMeshes(m_avatar);

        foreach (var headMesh in m_avatar.headMeshes)
            headMesh.updateWhenOffscreen = true;
        
        foreach (var hairMesh in m_avatar.hairMeshes)
            hairMesh.updateWhenOffscreen = true;

        foreach (var bodyMesh in m_avatar.bodyMeshes)
            bodyMesh.updateWhenOffscreen = true;
        
        m_microphone?.SetAvatar(m_avatar);

        m_avatar.gameObject.SetActive(true);

        m_proxy.name = $"{m_avatarCrate.name} - Proxy";
        m_proxy.SetActor(this);

        m_proxy.PoseAtEnd();
    }

    public void SwitchToActor(Actor actor)
    {
        m_avatar.gameObject.SetActive(false);
        actor.m_avatar.gameObject.SetActive(true);
    }

    public void OwnProp(Prop prop)
    {
        prop.SetActor(this);
        m_ownedProps.Add(prop);
    }

    public void DisownProp(Prop prop)
    {
        prop.SetActor(null);
        m_ownedProps.Remove(prop);
    }

    public override void Delete()
    {
        var ownedProps = m_ownedProps.ToList();
        
        foreach (var ownedProp in ownedProps)
        {
            ownedProp.DeleteAllFrames();
            PropBuilder.RemoveProp(ownedProp.Proxy.Entity);
        }

        ActorFrameManager.RemoveFrameFromActor(m_proxy);
        MarkerManager.RemoveMarkerFromActor(m_proxy);

        DestroyProxy();
        
        m_ownedProps.Clear();
        m_body?.Delete();
        m_body = null;
        GameObject.Destroy(m_avatar.gameObject);
        
        if (m_microphone)
            GameObject.Destroy(m_microphone.gameObject);
        
        m_microphone = null;
        m_frames.Clear();
    }

    public void SetHidden(bool hidden)
    {
        m_hidden = hidden;
        m_proxy.OnHidden();
    }

    public void Show()
    {
        Avatar.gameObject.SetActive(true);
    }

    public void Hide()
    {
        Avatar.gameObject.SetActive(false);
    }

    public void ParentToSeat(MarrowSeat seat)
    {
        m_activeSeat = seat;

        Transform pelvis = m_clonedRigBones[(int)HumanBodyBones.Hips];

        m_lastPelvisParent = pelvis.GetParent();

        Vector3 seatOffset = new Vector3(seat._buttOffset.x, Mathf.Abs(seat._buttOffset.y) * m_avatar.heightPercent, seat._buttOffset.z);

        pelvis.SetParent(seat.transform);

        pelvis.position = seat.buttTargetInWorld;
        pelvis.localPosition = seatOffset;
    }

    public void UnparentSeat()
    {
        m_activeSeat = null;
        Transform pelvis = m_clonedRigBones[(int)HumanBodyBones.Hips];
        pelvis.SetParent(m_lastPelvisParent);
    }

    private void ShowHairMeshes(MarrowAvatar avatar)
    {
        if(avatar == null)
        {
            Logging.ErrorDebug("ShowHairMeshes: Avatar doesn't exist!");
        }

        if(avatar.hairMeshes.Count == 0 || avatar.hairMeshes == null)
        {
            Logging.WarnDebug("ShowHairMeshes: No hair meshes to clone.");
        }

        foreach (var mesh in avatar.hairMeshes)
        {
            if(mesh == null)
            {
                continue;
            }

            mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        }
    }
    
    //
    // Enums
    //
    public enum VersionNumber : short
    {
        V1
    }
    
    //
    // IBinaryData
    //
    public byte[] ToBinary()
    {
        // TODO: Keep this up to date

        List<byte> bytes = new List<byte>();
        
        // The header contains the following data
        //
        // version: u16
        // barcode_size: i32
        // barcode : utf-8 string
        // num_frames : u32
        //
        // Below the header is the following
        //
        // num_frames FrameGroup blocks
        //
        bytes.AddRange(BitConverter.GetBytes((short)VersionNumber.V1));
        
        byte[] encodedBarcode = Encoding.UTF8.GetBytes(m_avatarCrate._barcode._id);
        bytes.AddRange(BitConverter.GetBytes(encodedBarcode.Length));
        bytes.AddRange(encodedBarcode);
        bytes.AddRange(BitConverter.GetBytes(m_frames.Count));

        foreach (FrameGroup group in m_frames)
            bytes.AddRange(group.ToBinary());

        return bytes.ToArray();
    }

    public void FromBinary(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        
        // Check the version number
        ushort version = reader.ReadUInt16();

        if (version != (short)VersionNumber.V1)
            throw new Exception($"Unsupported version type! Value was {version}");

        // Deserialize
        if (version == (short)VersionNumber.V1)
        {
            // How long is the string?
            int strlen = reader.ReadInt32();
            byte[] strBytes = reader.ReadBytes(strlen);
            string barcodeID = Encoding.UTF8.GetString(strBytes);

            Barcode avatarBarcode = new Barcode(barcodeID);
            AssetWarehouse.Instance.TryGetCrate(avatarBarcode, out AvatarCrate crate);
            m_avatarCrate = crate;
#if DEBUG
            Logging.MsgDebug($"[ACTOR]: Barcode: {m_avatarCrate}");
#endif
            
            // Then deserialize the frames
            int numFrames = reader.ReadInt32();

#if DEBUG
            Logging.MsgDebug($"[ACTOR]: NumFrames: {numFrames}");
#endif
            
            FrameGroup[] frameGroups = new FrameGroup[numFrames];

            for (int f = 0; f < numFrames; f++)
            {
                frameGroups[f] = new FrameGroup();
                frameGroups[f].FromBinary(stream);
            }

            m_frames = new List<FrameGroup>(frameGroups);
        }
    }

    // TADB - Tracked Actor Data Block
    public uint GetBinaryID() => 0x41435452;
}
