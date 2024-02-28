using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return GetInstance();
        }
    }

    private static GameManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<GameManager>();
            if (_instance == null)
            {
                var go = new GameObject("[Singleton] GameManager");
                _instance = go.AddComponent<GameManager>();
            }
        }
        return _instance;
    }
    #endregion

    #region Variable
    private TemperatureManager _temperature = new();
    private ArchitectureManager _architecture = new();
    private DayCycle _daycycle = new();
    private Season _season = new();
    private Disaster _disaster = new();
    private ObjectManager _objectManager = new();
    private ArtifactCreator _artifactCreator = new();
    private WorldNavMeshBuilder _worldNavmeshBuilder;
    private ResourceObjectSpawner _resourceObjectSpawner = new();
    private PlayerHelper _playerHelper = new();
    #endregion

    #region Property 
    public static TemperatureManager Temperature => _instance._temperature;
    public static ArchitectureManager Architecture => _instance._architecture;
    public static ObjectManager ObjectManager => _instance._objectManager;
    public static DayCycle DayCycle => _instance._daycycle;
    public static Season Season => _instance._season;
    public static ArtifactCreator ArtifactCreator => _instance._artifactCreator;
    public static ResourceObjectSpawner ResourceObjectSpawner => _instance._resourceObjectSpawner;
    public World World { get; private set; }
    public Player Player { get; set; }
    public WorldNavMeshBuilder WorldNavMeshBuilder
    {
        get
        {
            if (_worldNavmeshBuilder == null)
                _worldNavmeshBuilder = new GameObject(nameof(WorldNavMeshBuilder)).AddComponent<WorldNavMeshBuilder>();
            return _worldNavmeshBuilder;
        }
    }
    public Island IceIsland { get; private set; }
    public Island CenterIsland { get; private set; }
    public Island FireIsland { get; private set; }
    public MonsterWave MonsterWave { get; private set; }
    public bool IsRunning { get; private set; } = false;
    #endregion

    private int _saveInterval = 12;
    private int _saveStack = 0;
    public event Action OnSaveCallback;

    public void StartGame()
    {
        Player.Load();
        Player.ConditionHandler.HP.OnBelowedToZero += (() => { IsRunning = false; });
        InitIslands();

        InitSystems();
        BindEvent();

        Managers.Sound.PlayIslandBGM(Player.StandingIslandName);

        DayCycle.Start();
        IsRunning = true;
    }

    private void InitSystems()
    {
        MonsterWave = new();
        _disaster.Init();
        _daycycle.Init();
        _season.Init();
        _playerHelper.Init();
        _resourceObjectSpawner.Init();
        _artifactCreator.Init();
        _temperature.Init();
        _architecture.Init();
    }

    private void BindEvent()
    {
        _disaster.BindEvent();
        _season.BindEvent();
        _artifactCreator.BindEvent();
        _playerHelper.BindEvent();
        _temperature.BindEvent();
        _architecture.BindEvent();
        _daycycle.BindEvent();
        MonsterWave.BindEvent();

        DayCycle.OnTimeUpdated += SaveCallback;
    }

    public void GenerateWorldAsync(Action<float, string> progressCallback = null, Action completedCallback = null)
    {
        World = new GameObject("@World").AddComponent<World>();
        World.GenerateWorldAsync(progressCallback, completedCallback);
    }

    public void GenerateNavMeshAsync(bool autoUpdate = false, Action<AsyncOperation> callback = null)
    {
        WorldNavMeshBuilder.AutoUpdate = autoUpdate; // 주기적으로 업데이트 X. 게임 초기화 시 한 번만 생성합니다.
        WorldNavMeshBuilder.GenerateNavMeshAsync(callback);
    }

    public void InitIslands()
    {
        IceIsland = new Island(Managers.Resource.GetCache<IslandSO>("GlacierIsland.data"));
        IceIsland.Load();

        CenterIsland = new Island(Managers.Resource.GetCache<IslandSO>("CenterIsland.data"));
        CenterIsland.Load();

        FireIsland = new Island(Managers.Resource.GetCache<IslandSO>("LavaIsland.data"));
        FireIsland.Load();
    }

    private void SaveCallback()
    {
        if (!IsRunning) return;
        ++_saveStack;
        if (_saveStack / _saveInterval == 1)
        {
            OnSaveCallback?.Invoke();
            _saveStack = 0;
        }
    }

    public void SaveGameData()
    {
        if (!IsRunning) return;
        OnSaveCallback?.Invoke();
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }
}