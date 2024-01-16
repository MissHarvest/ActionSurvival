// 작성 날짜 : 2024. 01. 10
// 작성자 : Park Jun Uk

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureManager
{
    // 현재 온도 { get; set; }
    public float Temperature { get; private set; } = 8.0f;

    public event Action<float> OnChanged;

    public void Init(GameManager gameManager)
    {
        // 시간대 콜백

        // 몬스터 비율 콜백
    }

    private void OnMonsterSpawned(float ratio)
    {

    }

    private void OnMorningCame()
    {
        // 2분?
    }

    private void OnEveningCame()
    {
        // 
    }

    private void OnNightCame()
    {
        // 
    }
}
