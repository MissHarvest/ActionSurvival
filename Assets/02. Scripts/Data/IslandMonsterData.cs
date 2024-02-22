using UnityEngine;

// 2024. 02. 22 Park Jun Uk
[System.Serializable]
public class IslandMonsterData
{
    [field : SerializeField] public IslandMonsterSpawnData SpawnData { get; private set; }
    [field : SerializeField] public IslandMonsterDistribution Distribution { get; private set; }
    [field : SerializeField] public GameObject BossMonster { get; private set; }
}
