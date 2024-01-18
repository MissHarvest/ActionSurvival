using System;
using UnityEngine;

public class GameManager
{
    public Player Player;
    public TemperatureManager Temperature { get; private set; } = new TemperatureManager();
    public DayCycle DayCycle { get; private set; } = new DayCycle();

    public World World { get; private set; }

    public Island IceIsland = new Island(new IsLandProperty(Vector3.zero));
    public Island CenterIsland = new Island(new IsLandProperty(new Vector3(-400, 0, 0)));
    public Island FireIsLand = new Island(new IsLandProperty(new Vector3(400, 0, 0)));

    public void Init()
    {
        DayCycle.Init();
        Temperature.Init(this);

        IceIsland.AddMonsterType(new string[][]{
            new string[] { "Skeleton" },
            new string[] { "Skeleton" },
            new string[] { "Skeleton" }
            });

        CenterIsland.AddMonsterType(new string[][]{
            new string[] { "Skeleton" },
            new string[] { "Skeleton" },
            new string[] { "Skeleton" }
            });

        FireIsLand.AddMonsterType(new string[][]{
            new string[] { "Skeleton" },
            new string[] { "Skeleton" },
            new string[] { "Skeleton" }
            });

        IceIsland.CreateMonsters();
        CenterIsland.CreateMonsters();
        FireIsLand.CreateMonsters();
    }

    public void GenerateWorldAsync(Action<float, string> progressCallback = null, Action completedCallback = null)
    {
        World = new GameObject("@World").AddComponent<World>();
        World.GenerateWorldAsync(progressCallback, completedCallback);
    }
}