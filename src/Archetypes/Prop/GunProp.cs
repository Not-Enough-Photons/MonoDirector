using System.Text;
using NEP.MonoDirector.Data;

using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.PuppetMasta;
using NEP.MonoDirector.Core;
using UnityEngine;

namespace NEP.MonoDirector.Archetypes;

public class GunProp : Prop, IBinaryData
{
    public Gun Gun { get => m_gun; }

    public override Type PropType => Type.Gun;

    private Gun m_gun;

    public override void OnSceneBegin()
    {
        base.OnSceneBegin();

        foreach(ActionFrame actionFrame in m_actions)
            actionFrame.Reset();
    }

    public void GunFakeFire()
    {
        m_gun.cartridgeState = Gun.CartridgeStates.SPENT;
        m_gun.UpdateArt();
        
        MuzzleFlash();
        //EjectCasing();
        m_gun.gunSFX.GunShot();
    }

    public void SetGun(Gun gun)
    {
        this.m_gun = gun;
        SlideVirtualController slideVirtualController = gun.slideVirtualController;
        if (slideVirtualController != null)
        {
            gun.slideVirtualController.OnSlideGrabbed = gun.slideVirtualController.OnSlideGrabbed + new System.Action(() => RecordSlideGrabbed());
            gun.slideVirtualController.OnSlideReleased = gun.slideVirtualController.OnSlideReleased + new System.Action(() => RecordSlideReleased());
            gun.slideVirtualController.OnSlidePulled = gun.slideVirtualController.OnSlidePulled + new System.Action(() => RecordSlidePulled());
            gun.slideVirtualController.OnSlideUpdate = gun.slideVirtualController.OnSlideUpdate + new System.Action<float>((float perc) => RecordSlideUpdate(perc));
            gun.slideVirtualController.OnSlideReturned = gun.slideVirtualController.OnSlideReturned + new System.Action(() => RecordSlideReturned());
        }
        if(gun.internalMagazine != null)
        {
            _prevInternalMagazineAmmoCount = gun.MagazineState.AmmoCount;
            gun.MagazineState.onAmmoChange = gun.MagazineState.onAmmoChange + new System.Action<int>((int count) => OnAmmoChanged_InternalMagazine(count));
        }
    }

    private void MuzzleFlash()
    {
        Spawnable muzzleFlashSpawnable = new Spawnable()
        {
            crateRef = m_gun.muzzleFlareSpawnable.crateRef
        };
        AssetSpawner.Register(muzzleFlashSpawnable);

        AssetSpawner.Spawn(muzzleFlashSpawnable,
            m_gun.firePointTransform.position,
            m_gun.firePointTransform.rotation,
            new Il2CppSystem.Nullable<Vector3>(Vector3.one),
            null,
            false,
            new Il2CppSystem.Nullable<int>(),
            null,
            null);
    }

    private void EjectCasing()
    {
        Spawnable cartridgeSpawnable = new Spawnable()
        {
            crateRef = m_gun.defaultCartridge.cartridgeCaseSpawnable.crateRef
        };
        AssetSpawner.Register(cartridgeSpawnable);

        AssetSpawner.Spawn(cartridgeSpawnable,
            m_gun.shellSpawnTransform.position,
            m_gun.shellOrientationTransform.rotation);
    }

    public void InsertMagState(CartridgeData cartridgeData, MagazineData magazineData, int count)
    {
        if (m_gun.internalMagazine != null)
        {
            m_gun.MagazineState.Initialize(cartridgeData, count);
        }
        else
        {
            MagazineState magazineState = new MagazineState()
            {
                cartridgeData = cartridgeData,
                magazineData = magazineData
            };
            magazineState.Initialize(cartridgeData, count);
            m_gun.MagazineState = magazineState;
        }

        // gun.UpdateMagazineArt();
    }

    public void AddMagState(CartridgeData cartridgeData, int amount)
    {
        m_gun.MagazineState.AddCartridge(amount, cartridgeData);
        // gun.UpdateMagazineArt();
    }

    private int _prevInternalMagazineAmmoCount;
    public void OnAmmoChanged_InternalMagazine(int count)
    {
        if (_prevInternalMagazineAmmoCount < count)
        {
            int amount = count - _prevInternalMagazineAmmoCount;
            _prevInternalMagazineAmmoCount = count;
            CartridgeData cartridgeData = m_gun.MagazineState.GetCartridgeData();
            RecordAction((byte)ActionType.AmmoChanged, new System.Action(() => AddMagState(cartridgeData, amount)));
        }
    }

    public void RemoveMagState()
    {
        m_gun.MagazineState = null;
    }

    public void UpdateArt()
    {
        m_gun.UpdateArt();
    }

    public void RecordSlideGrabbed()
    {
        RecordAction((byte)ActionType.SlideGrabbed, () => m_gun.slideVirtualController.OnSlideGrabbed.Invoke());
    }
    public void RecordSlideReleased()
    {
        RecordAction((byte)ActionType.SlideReleased, () => m_gun.slideVirtualController.OnSlideReleased.Invoke());
    }
    public void RecordSlidePulled()
    {
        RecordAction((byte)ActionType.SlidePulled, () => m_gun.slideVirtualController.OnSlidePulled.Invoke());
    }
    public void RecordSlideUpdate(float perc)
    {
        RecordAction((byte)ActionType.SlideUpdate, () => m_gun.slideVirtualController.OnSlideUpdate.Invoke(perc));
    }
    public void RecordSlideReturned()
    {
        RecordAction((byte)ActionType.SlideReturned, () => m_gun.slideVirtualController.OnSlideReturned.Invoke());
    }
    
    public uint GetBinaryID()
    {
        throw new NotImplementedException();
    }

    public byte[] ToBinary()
    {
        // The header contains the following data
        //
        // version: u16
        // barcode_size: i32
        // barcode : utf-8 string
        // num_frames : u32
        // num_actions : u32

        using MemoryStream stream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(stream);
        
        //writer.Write(BitConverter.GetBytes((ushort)0x0));
        writer.Write((byte)PropType);
        writer.Write(BitConverter.GetBytes(m_barcode.Length));
        writer.Write(Encoding.UTF8.GetBytes(m_barcode));
        writer.Write(BitConverter.GetBytes(m_frames.Count));
        writer.Write(BitConverter.GetBytes(m_actions.Count));
        
        foreach (FrameGroup group in m_frames)
            writer.Write(group.ToBinary());
        
        foreach (ActionFrame action in m_actions)
            writer.Write(action.ToBinary());

        return stream.ToArray();
    }

    public void FromBinary(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        // How long is the string?
        int strlen = reader.ReadInt32();
        byte[] strBytes = reader.ReadBytes(strlen);
        string barcodeID = Encoding.UTF8.GetString(strBytes);

        m_barcode = barcodeID;
            
        // Then deserialize the frames
        int numFrames = reader.ReadInt32();
            
        // Then deserialize the action frames
        int numActionFrames = reader.ReadInt32();

#if DEBUG
        Logging.MsgDebug($"[PROP]: NumFrames: {numFrames}");
#endif

        FrameGroup[] frameGroups = new FrameGroup[numFrames];

        for (int f = 0; f < numFrames; f++)
        {
            frameGroups[f] = new FrameGroup();
            frameGroups[f].FromBinary(stream);
        }

        m_frames = new List<FrameGroup>(frameGroups);

        ActionFrame[] actionFrames = new ActionFrame[numActionFrames];

        for (int f = 0; f < numActionFrames; f++)
        {
            actionFrames[f] = new ActionFrame();
            ActionFrame action = actionFrames[f];
            action.FromBinary(stream);
            SetupAction(action);
        }

        m_actions = new List<ActionFrame>(actionFrames);
    }

    protected override void SetupAction(ActionFrame action)
    {
        base.SetupAction(action);

        switch ((ActionType)action.type)
        {
            case ActionType.Fire:
                action.action = GunFakeFire;
                break;
            case ActionType.RemoveMag:
                action.action = RemoveMagState;
                break;
            default:
                Logging.WarnDebug($"Case for action type {action.type.ToString()} was not implemented");
                break;
        }
    }
}
