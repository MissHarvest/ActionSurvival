using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "New Item/Consume", order = 1)]
public class ConsumeItemData : ItemData
{
    [field: SerializeField] public List<PlayerConditions> conditionModifier;

    public ConsumeItemData()
    {
        _maxStackCount = 20;
        registable = true;
    }
}
