using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.Warehouse;

using MelonLoader;
using MelonLoader.Utils;

using NEP.MonoDirector.Archetypes;
using NEP.MonoDirector.State;
using NEP.MonoDirector.Tools;
using NEP.MonoDirector.UI;
using NEP.MonoDirector.Yielding;

using UnityEngine;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Core;

public static class Director
{
    public static Film ActiveFilm { get => m_activeFilm; }
    public static Scene ActiveScene { get => m_activeScene; }

    public static string CurrentLevel { get => m_currentLevel; }
    
    public static PlayState PlayState { get => m_playState; }
    public static PlayState LastPlayState { get => m_lastPlayState; }
    public static CaptureState CaptureState { get => m_captureState; }

    public static event Action<Scene> OnSceneAdded;
    public static event Action<Scene> OnSceneRemoved;
    public static event Action<Scene> OnSceneSet;

    private static string m_currentLevel;

    private static Film m_activeFilm;
    private static Scene m_activeScene;

    private static PlayState m_playState = PlayState.Stopped;
    private static PlayState m_lastPlayState;
    private static CaptureState m_captureState = CaptureState.CaptureActor;

    private static int m_worldTick;

    internal static void Initialize()
    {
        Playback.Initialize();
        Recorder.Initialize();
        Caster.Initialize();

        Caster.OnActorRecasted += (_) => Record();
        
        Events.OnPrePlayback += () => SetPlayState(PlayState.Preplaying);
        Events.OnPreRecord += () => SetPlayState(PlayState.Prerecording);

        Events.OnPlay += () => SetPlayState(PlayState.Playing);
        Events.OnStartRecording += () => SetPlayState(PlayState.Recording);

        m_activeFilm = new Film();
        m_activeScene = new Scene();
        m_activeFilm.AddScene(m_activeScene);
        m_activeFilm.SetLevel(m_currentLevel);
    }

    internal static void Shutdown()
    {
        Playback.Shutdown();
        Recorder.Shutdown();
        Caster.OnActorRecasted -= (_) => Record();
        
        Events.OnPrePlayback -= () => SetPlayState(PlayState.Preplaying);
        Events.OnPreRecord -= () => SetPlayState(PlayState.Prerecording);

        Events.OnPlay -= () => SetPlayState(PlayState.Playing);
        Events.OnStartRecording -= () => SetPlayState(PlayState.Recording);
    }

    public static void Update()
    {
        if (!Settings.Debug.useKeys)
            return;

        float seekRate = Playback.PlaybackRate * Time.deltaTime;
        
        if (Input.GetKey(KeyCode.LeftArrow))
            Playback.Seek(-seekRate);

        if (Input.GetKey(KeyCode.RightArrow))
            Playback.Seek(seekRate);

        if (Input.GetKeyDown(KeyCode.F5))
            Save("test");

        if (Input.GetKeyDown(KeyCode.F6))
            Load("test");
    }

    public static void Play()
    {
        Playback.BeginPlayback();
    }

    public static void Pause()
    {
        SetPlayState(PlayState.Paused);
    }

    public static void Record()
    {
        Recorder.StartRecordRoutine();
    }

    public static void Recast(Actor actor)
    {
        Caster.RecastActor(actor);
        Record();
    }

    public static void Stop()
    {
        SetPlayState(PlayState.Stopped);
    }

    public static void AddScene(Scene scene)
    {
        m_activeFilm.AddScene(scene);
        OnSceneAdded?.Invoke(scene);
    }

    public static void RemoveScene(Scene scene)
    {
        // Removing the same stage
        if (m_activeScene.SceneIndex == scene.SceneIndex && m_activeFilm.Scenes.Count > 1)
        {
            if (m_activeScene.SceneIndex > 0)
                m_activeScene = m_activeFilm.Scenes[m_activeScene.SceneIndex - 1];
            else if (m_activeScene.SceneIndex == 0)
                m_activeScene = m_activeFilm.Scenes[m_activeScene.SceneIndex + 1];

            SetScene(m_activeScene);
        }

        var actors = m_activeScene.Actors.ToList();

        foreach (var actor in actors)
            RemoveActor(actor);
        
        m_activeFilm.RemoveScene(scene);
        
        // If the film is empty, add a new scene and set it.
        if (m_activeFilm.Empty)
        {
            Scene newScene = new Scene();
            AddScene(newScene);
            SetScene(newScene);
        }

        OnSceneRemoved?.Invoke(scene);
    }

    public static void SetFilm(Film film)
    {
        m_activeFilm = film;
        SetScene(m_activeFilm.Scenes.First());
    }
    
    public static void SetScene(Scene scene)
    {
        if (scene == null)
        {
            Logging.WarnDebug("Director.SetStage was called with a null stage!");
            return;
        }

        foreach (var actor in Caster.Cast)
        {
            actor.ActorBody.AllowCollisions(false);
            actor.Hide();
            MarkerManager.RemoveMarkerFromActor(actor.Proxy);
        }

        foreach (var prop in Caster.Props)
        {
            prop.Proxy.Hide();
            MarkerManager.RemoveMarkerFromProp(prop);
            PropFrameManager.RemoveFrameFromProp(prop);
        }

        Caster.ClearCast();
        Caster.ClearProps();
        Caster.ClearRecordProps();

        m_activeScene = scene;

        Caster.CastActors(scene.Actors.ToList());
        Caster.AddProps(scene.Props.ToList());

        foreach (var actor in Caster.Cast)
        {
            actor.ActorBody?.AllowCollisions(true);
            actor.Show();
            actor.OnSceneBegin();
            MarkerManager.AddMarkerToActor(actor.Proxy);
        }

        foreach (var prop in Caster.Props)
        {
            // idek why this happens
            if (prop.Proxy)
                prop.Proxy.Show();
            
            prop.OnSceneBegin();
            MarkerManager.AddMarkerToProp(prop);
            PropFrameManager.AddFrameToProp(prop);
        }

        OnSceneSet?.Invoke(scene);
    }

    public static void SelectActor(Actor actor) => Caster.SelectActor(actor);

    public static void DeselectActor(Actor actor) => Caster.DeselectActor(actor);

    public static void RemoveActor(Actor actor) => Caster.UncastActor(actor);

    #if DEBUG
    public static void Load(string name)
    {
        string input = Path.Combine(MelonEnvironment.UserDataDirectory, $"Not Enough Photons/MonoDirector/Films/{name}.mdf");

        if (!File.Exists(input))
        {
            Logging.Msg($"MonoDirector film {name}.mdf does not exist!");
            return;
        }
        
        FileStream stream = File.OpenRead(input);

        Film film = new Film();
        film.FromBinary(stream);
        
        stream.Dispose();
        stream.Close();

        if (film.LevelBarcode != m_currentLevel)
        {
            MelonCoroutines.Start(new WaitForLevelLoad(film.LevelBarcode).Then(() =>
            {
                PrepareFilm(film);
                SetFilm(film);
            }));
        }
        else
        {
            PrepareFilm(film);
            SetFilm(film);
        }
    }
    
    public static void Save(string name)
    {
        string output = Path.Combine(MelonEnvironment.UserDataDirectory, "Not Enough Photons/MonoDirector/Films");
        Directory.CreateDirectory(output);

        using FileStream stream = File.Create(Path.Combine(output, $"{name}.mdf"));
        using BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(m_activeFilm.ToBinary());
    }
    #endif
    
    public static void RemoveAllActors()
    {
        m_playState = PlayState.Stopped;

        for (int i = Caster.Cast.Count; i > 0; i--)
        {
            Caster.UncastActor(Caster.Cast[i]);
        }

        Caster.ClearCast();
    }

    public static void ClearScene()
    {
        RemoveAllActors();
        
        for (int i = Caster.Props.Count - 1; i > 0; i--)
        {
            Caster.RemoveProp(Caster.Props[i]);
            //GameObject.Destroy(Caster.Props[i]);
        }

        Caster.ClearProps();
    }

    public static void SetPlayState(PlayState state)
    {
        m_lastPlayState = m_playState;
        m_playState = state;
        Events.OnPlayStateSet?.Invoke(state);
    }

    public static void SetLevel(string levelBarcode)
    {
        m_currentLevel = levelBarcode;
    }

    private static void PrepareFilm(Film film)
    {
        foreach (var scene in film.Scenes)
            PrepareScene(scene);
    }

    private static void PrepareScene(Scene scene)
    {
        foreach (var actor in scene.Actors)
        {
            // Is the avatar installed?
            if (!actor.AvatarCrate)
                continue;
            
            MelonCoroutines.Start(new WaitForAvatarSpawn<MarrowAvatar>(actor.AvatarCrate).Then(avatar =>
            {
                actor.CreateProxy(avatar);
                actor.UpdateClone();
            }));
        }
        
        foreach (var prop in scene.Props)
        {
            SpawnableCrateReference crateRef = new SpawnableCrateReference(new Barcode(prop.Barcode));

            // Is the spawnable installed?
            if (!crateRef.Crate)
                continue;
                
            Spawnable spawnable = new Spawnable()
            {
                crateRef = crateRef
            };
            
            AssetSpawner.Register(spawnable);
            
            MelonCoroutines.Start(new WaitForAssetSpawn<MarrowEntity>(spawnable, Vector3.zero, Quaternion.identity).Then(entity =>
            {
                prop.CreateProxy(entity);
                prop.OnSceneBegin();

                if (prop.PropType == Prop.Type.Gun)
                {
                    GunProp gun = prop as GunProp;
                    gun.SetGun(prop.Proxy.GetComponent<Gun>());
                }
            }));
        }
        
        foreach (var entity in scene.Entities)
        {
            SpawnableCrateReference crateRef = new SpawnableCrateReference(new Barcode(entity.Barcode));
            
            // Is the spawnable installed?
            if (!crateRef.Crate)
                continue;
            
            Spawnable spawnable = new Spawnable()
            {
                crateRef = crateRef
            };
            
            AssetSpawner.Register(spawnable);
            
            MelonCoroutines.Start(new WaitForAssetSpawn<MarrowEntity>(spawnable, entity.Position, entity.Rotation).Then(ent =>
            {
                if (entity.EntityType == Entity.Type.Light)
                {
                    LightEntity light = (LightEntity)entity;

                    if (!light.Directional)
                    {
                        if (!ent.TryGetComponent(out OmniLight omniLight))
                            return;
                        
                        omniLight.LoadFromEntity(light);
                    }
                    else
                    {
                        if (!ent.TryGetComponent(out SpotLight spotLight))
                            return;
                        
                        spotLight.LoadFromEntity(light);
                    }
                }
                else if (entity.EntityType == Entity.Type.Sound)
                {
                    SoundEntity sound = (SoundEntity)entity;
                    
                    if (!ent.TryGetComponent(out SoundSource soundSource))
                        return;
                        
                    soundSource.LoadFromEntity(sound);
                }
            }));
        }
    }
}
