using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager
{
    public Player Player;
    public TemperatureManager Temperature { get; private set; } = new TemperatureManager();
    public DayCycle DayCycle { get; private set; } = new DayCycle();

    public World World { get; private set; }

    public Island IceIsland = new Island(new IsLandProperty(new Vector3(-400, 0, 0)));
    public Island CenterIsland = new Island(new IsLandProperty(Vector3.zero));
    public Island FireIsland = new Island(new IsLandProperty(new Vector3(400, 0, 0)));

    public MonsterWave MonsterWave { get; private set; }

    public void Init()
    {
        MonsterWave = new MonsterWave();

        DayCycle.Init();
        DayCycle.OnEveningCame += SpawnMonster;
        DayCycle.OnNightCame += StartMonsterWave;

        Temperature.Init(this);

        //InitIslands();
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

    private void InitIslands()
    {
        // MonsterGroup 을 만들어서 넘기는거..?
        IceIsland.AddMonsterType(new string[][]{
            new string[] { "IceSkeleton", "IceBat", "IceSwarm", "IceRabbitMon", "Beholder" },
            new string[] { "Fuga" , "BlueMetalon" , "IceElemental" },
            new string[] { "BlueSoulEater" },
            });

        CenterIsland.AddMonsterType(new string[][]{
            new string[] { "Skeleton" },
            new string[] { "Skeleton" },
            new string[] { "Skeleton" }
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
}