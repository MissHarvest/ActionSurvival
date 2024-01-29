// 작성 날짜 : 2024. 01. 12
// 작성자 : Park Jun Uk

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterMovementData
{
    [field: SerializeField] public float BaseSpeed { get; private set; }
    [field: SerializeField] public float WalkSpeedModifier { get; private set; }
    [field: SerializeField] public float ChaseSpeedModifier { get; private set; }
    [field: SerializeField] [field : Range(5.0f, 10.0f)] public float PatrolRadius { get; private set; }
}
