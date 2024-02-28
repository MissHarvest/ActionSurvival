// 작성 날짜 : 2024. 01. 12
// 작성자 : Park Jun Uk

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterAttackData
{
    [field: SerializeField] public int Atk { get; set; }
    [field : SerializeField] public float DetectionDistance { get; private set; }
    [field : SerializeField] public float DefaultDetectionDistModifier { get; private set; }
    [field : SerializeField] public float ChaseDectionDistModifier { get; private set; }
    [field : SerializeField] public float AttackalbeDistance { get; private set; }
    [field: SerializeField] public float AttackInterval { get; private set; }
    [field: SerializeField] public float DelayedTimeColliderActivated { get; private set; }
    [field: SerializeField] public float TimeColliderInactivated { get; private set; }
}
