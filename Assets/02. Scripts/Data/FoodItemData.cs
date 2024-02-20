using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "New Item/Food", order = 1)]
public class FoodItemData : ItemData
{
    [field: SerializeField] public List<PlayerConditions> conditionModifier;

    public FoodItemData()
    {
        _maxStackCount = 20;
        registable = true;
    }
}
