using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager
{
    public Player Player;
    public TemperatureManager Temperature { get; private set; } = new TemperatureManager();
    public ArchitectureManager Architecture { get; private set; } = new();
    public ObjectManager ObjectManager { get; private set; } = new();
    public DayCycle DayCycle { get; private set; } = new DayCycle();

    public event Action OnSaveCallback;

    public World World { get; private set; }
    private ResourceObjectSpawner _resourceObjectSpawner = new();

    public Island IceIsland = new Island(new IslandProperty(new Vector3(317, 0, 0), nameof(IceIsland)));
    public Island CenterIsland = new Island(new IslandProperty(Vector3.zero, nameof(CenterIsland)));
    public Island FireIsland = new Island(new IslandProperty(new Vector3(-317, 0, 0), nameof(FireIsland)));

    public MonsterWave MonsterWave { get; private set; }

    public bool IsRunning { get; private set; } = false;

    public void Init()
    {
        MonsterWave = new MonsterWave();

        DayCycle.Init();
        DayCycle.OnEveningCame += SpawnMonster;
        DayCycle.OnNightCame += StartMonsterWave;
        
        Architecture.Init();
        
        DayCycle.OnMorningCame += SaveCallback;

        Temperature.Init(this);

        _resourceObjectSpawner.Initialize();
        
        InitIslands();
        Managers.Sound.PlayIslandBGM();

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
        MonsterWave.Start();
    }

    public void GenerateWorldAsync(Action<float, string> progressCallback = null, Action completedCallback = null)
    {
        World = new GameObject("@World").AddComponent<World>();
        World.GenerateWorldAsync(progressCallback, completedCallback);
    }

    public void InitIslands()
    {
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

        IceIsland.CreateMonsters();
        CenterIsland.CreateMonsters();
        FireIsland.CreateMonsters();
    }

    private void SaveCallback()
    {
        OnSaveCallback?.Invoke();
    }
}