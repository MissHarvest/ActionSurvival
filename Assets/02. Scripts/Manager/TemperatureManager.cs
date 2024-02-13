// 작성 날짜 : 2024. 01. 10
// 작성자 : Park Jun Uk

using System;
using UnityEngine;

public class TemperatureManager
{
    private float _fireIslandTemperature;
    private float _iceIslandTemperature;
    private float _environmentTemperature;

    private Island _iceIsland;
    private Island _fireIsland;
    private float _islandDistance;

    public event Action<float> OnChanged;

    public void Init(GameManager gameManager)
    {
        _iceIsland = gameManager.IceIsland;
        _fireIsland = gameManager.FireIsland;
        _islandDistance = Vector3.Distance(_iceIsland.Position, _fireIsland.Position);

        // TEST
        _fireIslandTemperature = 40f;
        _iceIslandTemperature = -35f;

        // 시간대 콜백
        gameManager.DayCycle.OnMorningCame += OnMorningCame;
        gameManager.DayCycle.OnEveningCame += OnEveningCame;
        gameManager.DayCycle.OnNightCame += OnNightCame;

        gameManager.World.OnWorldUpdated += () => GetTemporature(gameManager.Player.transform.position);

        // 몬스터 비율 콜백
    }

    public float GetTemporature(Vector3 position)
    {
        float t = Vector3.Distance(_fireIsland.Position, position);
        t = Mathf.InverseLerp(0, _islandDistance, t);
        t = Mathf.Clamp01(t);
        Debug.Log(Mathf.Lerp(_fireIslandTemperature, _iceIslandTemperature, t) + _environmentTemperature);
        return Mathf.Lerp(_fireIslandTemperature, _iceIslandTemperature, t) + _environmentTemperature;
    }

    private void OnMonsterSpawned(float ratio)
    {

    }

    private void OnMorningCame()
    {
        _environmentTemperature = 12f;
    }

    private void OnEveningCame()
    {
        _environmentTemperature = 20f;
    }

    private void OnNightCame()
    {
        _environmentTemperature = 8f;
    }
}
