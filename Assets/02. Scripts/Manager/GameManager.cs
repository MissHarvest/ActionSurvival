using System;
using UnityEngine;

public class GameManager
{
    public Player Player;
    public TemperatureManager Temperature { get; private set; } = new TemperatureManager();
    public DayCycle DayCycle { get; private set; } = new DayCycle();

    public World World { get; private set; }

    public void Init()
    {
        DayCycle.Init();
        Temperature.Init(this);
    }

    public void GenerateWorldAsync(Action<float, string> progressCallback = null, Action completedCallback = null)
    {
        World = new GameObject("@World").AddComponent<World>();
        World.GenerateWorldAsync(progressCallback, completedCallback);
    }
}