using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster")]
public class MonsterSO : ScriptableObject
{
    [field : SerializeField] public MonsterMovementData MovementData { get; private set; }
    [field : SerializeField] public MonsterAttackData AttackData { get; private set; }
}
