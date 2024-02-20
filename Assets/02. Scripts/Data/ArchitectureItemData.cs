using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "New Item/Architecture", order = 4)]
public class ArchitectureItemData : ItemData
{
    public ArchitectureItemData()
    {
        _maxStackCount = 20;
        registable = true;
    }
}
