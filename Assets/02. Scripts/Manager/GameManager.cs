using System;
using UnityEngine;

public class GameManager
{
    public Player Player;
    public TemperatureManager Temperature { get; private set; } = new TemperatureManager();
    public DayCycle DayCycle { get; private set; } = new DayCycle();

    public World World { get; private set; }

    public Island IceIsland = new Island(new IsLandProperty(new Vector3(-400, 0, 0)));
    public Island CenterIsland = new Island(new IsLandProperty(Vector3.zero));
    public Island FireIsLand = new Island(new IsLandProperty(new Vector3(400, 0, 0)));

    public void Init()
    {
        DayCycle.Init();
        Temperature.Init(this);

        InitIsLands();
    }

    public void GenerateWorldAsync(Action<float, string> progressCallback = null, Action completedCallback = null)
    {
        World = new GameObject("@World").AddComponent<World>();
        World.GenerateWorldAsync(progressCallback, completedCallback);
    }

    private void InitIsLands()
    {
        IceIsland.AddMonsterType(new string[][]{            
            new string[] { "BlueSoulEater" },
            new string[] { "Fuga" , "BlueMetalon" , "IceElemental" },
            new string[] { "IceSkeleton", "IceBat", "IceSwarm", "IceRabbitMon", "Beholder" },
            });

        CenterIsland.AddMonsterType(new string[][]{
            new string[] { "Skeleton" },
            new string[] { "Skeleton" },
            new string[] { "Skeleton" }
            });

        FireIsLand.AddMonsterType(new string[][]{            
            new string[] { "RedSoulEater" },
            new string[] { "RedFuga" , "FireElemental" , "RedMetalon" },
            new string[] { "FireSkeleton", "FireBat", "FireRabbitMon", "FireSwarm", "Slime" },
            });

        IceIsland.CreateMonsters();
        CenterIsland.CreateMonsters();
        FireIsLand.CreateMonsters();
    }
}