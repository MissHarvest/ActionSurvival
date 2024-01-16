using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalMovementData
{
    [field: SerializeField] public float BaseSpeed { get; private set; }
    [field: SerializeField] public float WalkSpeedModifier { get; private set; }
    [field: SerializeField] public float FleeSpeedModifier { get; private set; }
    [field: SerializeField][field: Range(5.0f, 10.0f)] public float PatrolRadius { get; private set; }
}
