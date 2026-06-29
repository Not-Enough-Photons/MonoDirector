using Il2CppTMPro;
using MelonLoader;
using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using UnityEngine;

namespace NEP.MonoDirector.Tools;

[RegisterTypeInIl2Cpp]
public class SoundSource(IntPtr ptr) : PointToolEntity(ptr)
{
    public AudioClip Clip { get => m_clip; }

    protected AudioSource m_source;
    protected SoundEntity m_soundEntity;
    protected AudioClip m_clip;
    protected TextMeshPro m_nameText;
    private SoundSourceTether m_tether;
    private SoundVolumeGizmo m_volumeGizmo;
    private LineRenderer m_lineRenderer;
    private GameObject m_dial;

    private static int m_instance_count = 0;
    
    protected override void Awake()
    {
        base.Awake();

        m_source = GetComponent<AudioSource>();
        
        m_nameText = transform.Find("SoundName").GetComponent<TextMeshPro>();

        Transform tether = transform.Find("TetherGizmo");
        m_tether = tether.GetComponent<SoundSourceTether>();

        m_volumeGizmo = transform.Find("VolumeDial/Gizmo").GetComponent<SoundVolumeGizmo>();

        m_lineRenderer = transform.Find("Line").GetComponent<LineRenderer>();
        m_dial = transform.Find("VolumeDial/Quad").gameObject;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        // Instead of creating a new SoundEntity,
        // use an existing one from the active scene.
        int entityCount = Director.ActiveScene.Entities.Count;
        if (entityCount > 0)
        {
            for (int i = 0; i < entityCount; i++)
            {
                Entity entity = Director.ActiveScene.Entities[i];

                if (!entity.AssociatedTool && entity.EntityType == Entity.Type.Sound)
                {
                    entity.AssociateTool(this);
                    m_soundEntity = (SoundEntity)entity;
                }
            }
        }
        else
        {
            m_soundEntity = new SoundEntity();
            m_soundEntity.SetBarcode(m_poolee.SpawnableCrate._barcode._id);
        
            if (m_source.spatialBlend > 0f)
                m_soundEntity.SetIs3D(true);
            else
                m_soundEntity.SetIs3D(false);
        
            Director.ActiveScene.AddEntity(m_soundEntity);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        m_nameText.text = "N/A";
        
        m_soundEntity.AssociateTool(null);
        Director.ActiveScene.RemoveEntity(m_soundEntity);
        m_soundEntity = null;
    }
    
    public void LoadFromEntity(SoundEntity soundEntity)
    {
        m_soundEntity = soundEntity;

        if (!WarehouseLoader.soundTable.TryGetValue(m_soundEntity.SoundName, out AudioClip clip))
            return;
        
        LinkSound(clip);
    }

    protected override void Show()
    {
        base.Show();
        m_nameText.gameObject.SetActive(true);
        m_lineRenderer.enabled = true;
        m_tether.Show();
        m_volumeGizmo.Show();
        m_dial.SetActive(true);
    }

    protected override void Hide()
    {
        base.Hide();
        m_nameText.gameObject.SetActive(false);
        m_lineRenderer.enabled = false;
        m_tether.Hide();
        m_volumeGizmo.Hide();
        m_dial.SetActive(false);
    }

    private void Update()
    {
        float distance = Vector3.Distance(m_tether.transform.position, transform.position);

        m_lineRenderer.SetPosition(1, m_tether.transform.localPosition);

        m_soundEntity.SetPosition(transform.position);
        m_soundEntity.SetRotation(transform.rotation);
        m_soundEntity.SetVolume(m_volumeGizmo.Volume);
        
        m_source.volume = m_soundEntity.Volume;
    }

    private void OnTriggerEnter(Collider other)
    {
        SoundHolder soundHolder = other.GetComponent<SoundHolder>();

        if (soundHolder == null)
            return;

        LinkSound(soundHolder.GetSound());
        soundHolder.gameObject.SetActive(false);
        FeedbackSFX.LinkAudio();
    }

    private void LinkSound(AudioClip clip)
    {
        m_source.clip = clip;
        m_clip = m_source.clip;
        m_nameText.text = m_clip.name;
        m_soundEntity.SetSoundName(m_clip.name);
    }

    protected override void OnStartPlayback() => m_source.Play();

    protected override void OnStopPlayback() => m_source.Stop();

    protected override void OnStartRecording() => m_source.Play();

    protected override void OnStopRecording() => m_source.Stop();

    public void Mute() => m_source.volume = 0f;
    public void Unmute() => m_source.volume = 1f;
}