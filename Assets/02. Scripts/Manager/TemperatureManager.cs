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

    // 시간대 가져오기


    // 자신의 상태 ( 존나 추움, 추움, 무난, 따듯, 더움 } 필요한가? 

    // 각 상태 변경에 따른 이벤트
    // 추워졌다. 따뜻해졌다. 더워졌다. 얼어죽을꺼같다.
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
