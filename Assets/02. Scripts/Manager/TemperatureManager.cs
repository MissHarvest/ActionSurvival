// 작성 날짜 : 2024. 01. 10
// 작성자 : Park Jun Uk

using System;
using UnityEngine;

public class TemperatureManager
{
    private float _environmentTemperature;
    private AnimationCurve _influenceCurve;

    private Island _iceIsland;
    private Island _fireIsland;
    private float[] _temperatureByTime = new float[3] { 8.0f, 3.0f, 0.0f };
    public event Action OnChanged;

    public void Init()
    {
        _iceIsland = GameManager.Instance.IceIsland;
        _fireIsland = GameManager.Instance.FireIsland;
        _influenceCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    }

    public void BindEvent()
    {
        GameManager.DayCycle.OnStarted += OnTimeInitialized;
        GameManager.DayCycle.OnMorningCame += OnMorningCame;
        GameManager.DayCycle.OnEveningCame += OnEveningCame;
        GameManager.DayCycle.OnNightCame += OnNightCame;
    }

    public float GetTemperature(Vector3 position)
    {
        float iceTemperature = GetTemperature(position, _iceIsland.Property);
        float fireTemperature = GetTemperature(position, _fireIsland.Property);
        return iceTemperature + fireTemperature + _environmentTemperature;
    }

    private float GetTemperature(Vector3 position, IslandProperty island)
    {
        float temperature = Vector3.Distance(position, island.Center);
        temperature = Mathf.InverseLerp(0f, GetInfluenceDistance(island), temperature);
        temperature = _influenceCurve.Evaluate(temperature) * island.Temperature;
        return temperature;
    }

    public float GetInfluenceDistance(IslandProperty island)
    {
        return island.Diameter * island.Influence;
    }

    public void OnTemperatureChange()
    {
        OnChanged?.Invoke();
    }

    public void OnTimeInitialized(DayInfo day)
    {
        SetEnviromentTemperature((int)day.zone);
    }

    private void OnMorningCame()
    {
        SetEnviromentTemperature(0);
    }

    private void OnEveningCame()
    {
        SetEnviromentTemperature(1);
    }

    private void OnNightCame()
    {
        SetEnviromentTemperature(2);
    }

    private void SetEnviromentTemperature(int timezone)
    {
        _environmentTemperature = _temperatureByTime[timezone];
    }
}
