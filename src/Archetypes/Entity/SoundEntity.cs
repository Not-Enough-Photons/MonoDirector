using System.Text;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.Extensions;
using NEP.MonoDirector.Tools;
using UnityEngine;

namespace NEP.MonoDirector.Archetypes;

public class SoundEntity : Entity, IBinaryData
{
    public override Type EntityType => Type.Sound;

    public bool Is3D => m_is3D;
    public bool IsTethered => m_isTethered;
    public float Volume => m_volume;
    public string SoundName => m_soundName;
    
    private bool m_is3D;
    private bool m_isTethered;
    private float m_volume;
    private string m_soundName = string.Empty;
    
    public void SetIs3D(bool is3D)
    {
        m_is3D = is3D;
    }

    public void SetIsTethered(bool isTethered)
    {
        m_isTethered = isTethered;
    }

    public void SetVolume(float volume)
    {
        m_volume = volume;
    }

    public void SetSoundName(string name)
    {
        m_soundName = name;
    }
    
    public override void OnSceneBegin()
    {
        throw new NotImplementedException();
    }

    public override void OnSceneEnd()
    {
        throw new NotImplementedException();
    }

    public override void Perform()
    {
        throw new NotImplementedException();
    }

    public override void Record()
    {
        throw new NotImplementedException();
    }

    public override void RecordAction(byte type, Action action)
    {
        throw new NotImplementedException();
    }

    public override void Delete()
    {
        throw new NotImplementedException();
    }
    
    public byte[] ToBinary()
    {
        using MemoryStream stream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(stream);
        
        writer.Write((byte)EntityType);
        writer.Write(m_barcode.Length);
        writer.Write(Encoding.UTF8.GetBytes(m_barcode));
        writer.WriteVector3(m_position);
        writer.WriteQuaternion(m_rotation);
        writer.Write(m_is3D);
        writer.Write(m_isTethered);
        writer.Write(m_volume);
        writer.Write(m_soundName.Length);
        writer.Write(Encoding.UTF8.GetBytes(m_soundName));

        return stream.ToArray();
    }

    public void FromBinary(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        int barcodeSize = reader.ReadInt32();
        byte[] barcodeBytes = reader.ReadBytes(barcodeSize);
        m_barcode = Encoding.UTF8.GetString(barcodeBytes);
        
        m_position = reader.ReadVector3();
        m_rotation = reader.ReadQuaternion();

        byte is3D = reader.ReadByte();

        if (is3D == 0)
            m_is3D = false;
        else if (is3D == 1)
            m_is3D = true;

        byte isTethered = reader.ReadByte();

        if (isTethered == 0)
            m_isTethered = false;
        else if (isTethered == 1)
            m_isTethered = true;
        
        m_volume = reader.ReadSingle();
        
        int soundNameSize = reader.ReadInt32();
        byte[] soundNameBytes = reader.ReadBytes(soundNameSize);
        m_soundName = Encoding.UTF8.GetString(soundNameBytes);
    }

    public uint GetBinaryID()
    {
        throw new NotImplementedException();
    }
}