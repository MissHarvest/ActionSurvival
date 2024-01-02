using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "New Item/Equip", order = 2)]
public class EquipItemData : ItemData
{
    [Header("Extra")]
    public ItemParts part;
    public EquipItemData()
    {
        stackable = false;
        registable = false;
    }
}
