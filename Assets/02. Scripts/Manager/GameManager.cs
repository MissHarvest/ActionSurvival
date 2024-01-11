using System;
using UnityEngine;

public class GameManager
{
    public Player Player;
    public TemperatureManager Temperature { get; private set; } = new TemperatureManager();
    public DayCycle DayCycle { get; private set; } = new DayCycle();

    public void Init()
    {
        DayCycle.Init();
        Temperature.Init(this);
    }
}