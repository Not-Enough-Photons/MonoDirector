using System.Text;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.Extensions;
using UnityEngine;

namespace NEP.MonoDirector.Archetypes;

public class LightEntity : Entity, IBinaryData
{
    public override Type EntityType => Type.Light;

    public bool Directional => m_directional;
    
    public float Range => m_range;
    public float Intensity => m_intensity;
    public float Angle => m_angle;
    public Color Color => m_color;
    
    private bool m_directional;

    private float m_range;
    private float m_intensity;
    private float m_angle;
    private Color m_color;

    public void SetRange(float range)
    {
        m_range = range;
    }

    public void SetIntensity(float intensity)
    {
        m_intensity = intensity;
    }

    public void SetAngle(float angle)
    {
        m_angle = angle;
    }

    public void SetColor(Color color)
    {
        m_color = color;
    }

    public void SetDirectional(bool directional)
    {
        m_directional = directional;
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
        writer.Write(m_directional);
        writer.Write(m_range);
        writer.Write(m_intensity);
        writer.Write(m_angle);
        writer.WriteColor(m_color);
        
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

        byte isDirectional = reader.ReadByte();

        if (isDirectional == 0)
            m_directional = false;
        else if (isDirectional == 1)
            m_directional = true;

        m_range = reader.ReadSingle();
        m_intensity = reader.ReadSingle();
        m_angle = reader.ReadSingle();
        m_color = reader.ReadColor();
    }

    public uint GetBinaryID()
    {
        throw new NotImplementedException();
    }
}