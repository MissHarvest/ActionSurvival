using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager
{
    private WorldNavMeshBuilder _worldNavmeshBuilder;
    public Player Player;
    public TemperatureManager Temperature { get; private set; } = new TemperatureManager();
    public ArchitectureManager Architecture { get; private set; } = new();
    public ObjectManager ObjectManager { get; private set; } = new();
    public DayCycle DayCycle { get; private set; } = new DayCycle();

    public event Action OnSaveCallback;

    public World World { get; private set; }
    public WorldNavMeshBuilder WorldNavMeshBuilder
    {
        get
        {
            if (_worldNavmeshBuilder == null)
                _worldNavmeshBuilder = new GameObject(nameof(WorldNavMeshBuilder)).AddComponent<WorldNavMeshBuilder>();
            return _worldNavmeshBuilder;
        }
    }
    public ResourceObjectSpawner ResourceObjectSpawner { get; private set; } = new();

    public Island IceIsland;
    public Island CenterIsland;
    public Island FireIsland;

    public MonsterWave MonsterWave { get; private set; }

    public bool IsRunning { get; private set; } = false;

    public void Init()
    {
        Player.ConditionHandler.HP.OnBelowedToZero += (() => { IsRunning = false; });

        MonsterWave = new MonsterWave();

        DayCycle.Init();
        DayCycle.OnEveningCame += SpawnMonster;
        DayCycle.OnNightCame += StartMonsterWave;
        
        Architecture.Init();
        
        DayCycle.OnMorningCame += SaveCallback;

        ResourceObjectSpawner.Initialize();
        
        InitIslands();
        Temperature.Init(this);
        Managers.Sound.PlayIslandBGM(Player.StandingIslandName);

        IsRunning = true;
    }

    private void SpawnMonster()
    {
        int cnt = 1;
        switch(DayCycle.Season)
        {
            case Season.Summer:
                for(int i = 0; i < cnt; ++i)
                {
                    MonsterWave.AddOverFlowedMonster(FireIsland.Spawn());
                }
                break;

            case Season.Winter:
                for (int i = 0; i < cnt; ++i)
                {
                    MonsterWave.AddOverFlowedMonster(IceIsland.Spawn());
                }
                break;
        }
    }

    private void StartMonsterWave()
    {
        CoroutineManagement.Instance.StartCoroutine(MonsterWaveCoroutine());
    }

    private IEnumerator MonsterWaveCoroutine()
    {
        var delay = UnityEngine.Random.Range(0.0f, 60.0f);
        yield return new WaitForSeconds(delay);
        MonsterWave.Start();
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
        if (SaveGame.TryLoadJsonFile(SaveGame.SaveType.Runtime, "IslandProperties", out IslandPropertySaveData saveData))
        {
            IceIsland = new(saveData.dataList[0]);
            CenterIsland = new(saveData.dataList[1]);
            FireIsland = new(saveData.dataList[2]);
        }
        else
        {
            IceIsland = new Island(new IslandProperty(new Vector3(317, 0, 0), nameof(IceIsland), -25f, 1f));
            CenterIsland = new Island(new IslandProperty(Vector3.zero, nameof(CenterIsland), 0f, 0f));
            FireIsland = new Island(new IslandProperty(new Vector3(-317, 0, 0), nameof(FireIsland), 30f, 1f));
        }

        // MonsterGroup 을 만들어서 넘기는거..?
        IceIsland.AddMonsterType(new string[][]{
            new string[] { "IceSkeleton", "IceBat", "IceSwarm", "IceRabbitMon", "Beholder" },
            new string[] { "Fuga" , "BlueMetalon" , "IceElemental" },
            new string[] { "BlueSoulEater" },
            });

        CenterIsland.AddMonsterType(new string[][]{
            new string[] { "Skeleton", "Bat" },
            new string[] { "Skeleton", "Bat" },
            new string[] { "Skeleton", "Bat" },
            });

        FireIsland.AddMonsterType(new string[][]{
            new string[] { "FireSkeleton", "FireBat", "FireRabbitMon", "FireSwarm", "Slime" },
            new string[] { "RedFuga" , "FireElemental" , "RedMetalon" },
            new string[] { "RedSoulEater" },
            });

        FireIsland.BossName = "TerrorBringer";
        IceIsland.CreateMonsters();
        CenterIsland.CreateMonsters();
        FireIsland.CreateMonsters();

        OnSaveCallback += () => 
        {
            IslandPropertySaveData saveData = new()
            {
                dataList = new()
                {
                    IceIsland.Property,
                    CenterIsland.Property,
                    FireIsland.Property,
                },
            };
            string json = JsonUtility.ToJson(saveData);
            SaveGame.CreateJsonFile("IslandProperties", json, SaveGame.SaveType.Runtime);
        };
    }

    private void SaveCallback()
    {
        if (!IsRunning) return;
        OnSaveCallback?.Invoke();
    }
}