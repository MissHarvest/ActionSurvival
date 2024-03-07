using UnityEngine;

// 2024. 02. 22 Park Jun Uk
[CreateAssetMenu(fileName = "IslandSO", menuName = "New Island", order = 0)]
public class IslandSO : ScriptableObject
{
    [field:SerializeField] public IslandProperty Property { get; private set; }
    [field: SerializeField] public IslandMonsterData Monster { get; private set; }
}
