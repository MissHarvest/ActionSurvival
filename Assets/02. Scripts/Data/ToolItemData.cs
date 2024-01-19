using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToolItemData", menuName = "New Item/Weapon", order = 0)]
public class ToolItemData : EquipItemData
{
    public float maxDurability;
    public float currentDurability;

    public LayerMask targetLayers;
    public float range;
    public int attackPower;
    public int damage; // lgs 24.01.19
    public string targetTagName;

    public bool isWeapon;
    public bool isTwoHandedTool; // lgs
    public bool isTwinTool;
    
    public ToolItemData()
    {
        //stackable = false;
        registable = true;
        currentDurability = maxDurability;
        part = ItemParts.Hand;

        isTwoHandedTool = false;
    }
}
