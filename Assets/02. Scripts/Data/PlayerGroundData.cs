using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerGroundData
{
    [field: SerializeField][field: Range(0f, 25f)] public float BaseSpeed { get; private set; }
    [field: SerializeField][field: Range(0f, 25f)] public float BaseRotationDamping { get; private set; }
    [field: Header("IdleData")]

    [field: Header("WalkData")]
    [field: SerializeField][field: Range(0f, 2f)] public float WalkSpeedModifier { get;private set; }

    [field: Header("RunData")]
    [field: SerializeField][field: Range(0f, 2f)] public float RunSpeedModifier { get; private set; }
}
