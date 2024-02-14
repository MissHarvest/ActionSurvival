// 작성 날짜 : 2024. 01. 10
// 작성자 : Park Jun Uk

using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TemperatureManager
{
    private float _environmentTemperature;
    private AnimationCurve _influenceCurve;

    private Island _iceIsland;
    private Island _fireIsland;

    public event Action OnChanged;

    public void Init(GameManager gameManager)
    {
        OnMorningCame();
        _iceIsland = gameManager.IceIsland;
        _fireIsland = gameManager.FireIsland;

        _influenceCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        // 시간대 콜백
        gameManager.DayCycle.OnMorningCame += OnMorningCame;
        gameManager.DayCycle.OnEveningCame += OnEveningCame;
        gameManager.DayCycle.OnNightCame += OnNightCame;

        gameManager.World.OnWorldUpdated += () => GetTemperature(gameManager.Player.transform.position);
        OnChanged += () => GetTemperature(gameManager.Player.transform.position);

        // 몬스터 비율 콜백
    }

    public float GetTemperature(Vector3 position)
    {
        float iceTemperature = GetTemperature(position, _iceIsland.Property);
        float fireTemperature = GetTemperature(position, _fireIsland.Property);
        Debug.Log(new Vector3(iceTemperature, fireTemperature, _environmentTemperature) + " " + (iceTemperature + fireTemperature + _environmentTemperature));
        return iceTemperature + fireTemperature + _environmentTemperature;
    }

    private float GetTemperature(Vector3 position, IslandProperty island)
    {
        float temperature = Vector3.Distance(position, island.center);
        temperature = Mathf.InverseLerp(0f, GetInfluenceDistance(island), temperature);
        temperature = _influenceCurve.Evaluate(temperature) * island.Temperature;
        return temperature;
    }

    public float GetInfluenceDistance(IslandProperty island)
    {
        return island.diameter * island.Influence;
    }

    public void OnTemperatureChange()
    {
        OnChanged?.Invoke();
    }

    private void OnMonsterSpawned(float ratio)
    {

    }

    private void OnMorningCame()
    {
        _environmentTemperature = 8f;
    }

    private void OnEveningCame()
    {
        _environmentTemperature = 3f;
    }

    private void OnNightCame()
    {
        _environmentTemperature = 0f;
    }
}
