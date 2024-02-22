using UnityEngine;

// 2024. 02. 22 Park Jun Uk
[System.Serializable]
public class IslandMonsterSpawnData
{
    [field : SerializeField] public int Boundry { get; private set; } = 3;
    [field: SerializeField] public int CenterRadius { get; private set; } = 10;
    [field: SerializeField] public int MonsterCount { get; private set; } = 50;
    [field: SerializeField] public int Monsterinterval { get; private set; } = 5;
    [field: SerializeField] public int Interval { get; private set; } = 5;
    [field: SerializeField] public LayerMask UnSpawnableLayer { get; private set; }
}
