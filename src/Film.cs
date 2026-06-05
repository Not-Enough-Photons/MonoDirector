using System.Text;
using NEP.MonoDirector.Data;

namespace NEP.MonoDirector.Core;

public sealed class Film : IBinaryData
{
    public Film()
    {
        m_name = "Film";
        m_scenes = new List<Scene>();
        m_levelBarcode = string.Empty;
    }

    public Film(List<Scene> scenes)
    {
        m_name = "Film";
        m_scenes = scenes;
        m_levelBarcode = string.Empty;
    }

    public IReadOnlyList<Scene> Scenes => m_scenes.AsReadOnly();
    public string Name => m_name;
    public float Runtime => m_runtime;
    public bool Empty => Scenes.Count == 0;
    public string LevelBarcode => m_levelBarcode;
    
    private List<Scene> m_scenes;
    private string m_name;
    private float m_runtime;
    private string m_levelBarcode;

    public void AddScene(Scene scene)
    {
        scene.SetIndex(m_scenes.Count);
        m_scenes.Add(scene);
        m_runtime += scene.Duration;
    }

    public void RemoveScene(Scene scene)
    {
        m_scenes.Remove(scene);
        m_runtime -= scene.Duration;

        if (m_runtime <= 0f)
            m_runtime = 0f;

        for (int i = 0; i < m_scenes.Count; i++)
            m_scenes[i].SetIndex(i);
    }

    public void SetLevel(string barcode)
    {
        m_levelBarcode = barcode;
    }

    public uint GetBinaryID() => 0x46494c4d; // FILM

    public byte[] ToBinary()
    {
        // Header Information:
        // Version: u16
        // Film Name Length: u32
        // Film Name: UTF-8 String
        // Runtime: f32
        // Number of Scenes: u32
        // Level Barcode Length: u32
        // Level Barcode: UTF-8 String

        using MemoryStream stream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(stream);
        
        writer.Write(BitConverter.GetBytes((ushort)0x0));
        writer.Write(BitConverter.GetBytes(m_name.Length));
        writer.Write(Encoding.UTF8.GetBytes(m_name));
        writer.Write(BitConverter.GetBytes(m_runtime));
        writer.Write(BitConverter.GetBytes(m_scenes.Count));
        writer.Write(BitConverter.GetBytes(m_levelBarcode.Length));
        writer.Write(Encoding.UTF8.GetBytes(m_levelBarcode));
        
        foreach (var scene in m_scenes)
            writer.Write(scene.ToBinary());

        return stream.ToArray();
    }

    public void FromBinary(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        ushort version = reader.ReadUInt16();

        if (version == 0x0)
        {
            int filmNameLength = reader.ReadInt32();
            byte[] filmNameBytes = reader.ReadBytes(filmNameLength);
            string filmName = Encoding.UTF8.GetString(filmNameBytes);
            float runtime = reader.ReadSingle();
            int sceneCount = reader.ReadInt32();
            int levelBarcodeLength = reader.ReadInt32();
            byte[] levelBarcodeBytes = reader.ReadBytes(levelBarcodeLength);
            string levelBarcode = Encoding.UTF8.GetString(levelBarcodeBytes);

            m_name = filmName;
            m_runtime = runtime;
            m_scenes = new List<Scene>(sceneCount);
            m_levelBarcode = levelBarcode;

            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = new Scene();
                scene.FromBinary(stream);
                m_scenes.Add(scene);
            }
        }
    }
}
