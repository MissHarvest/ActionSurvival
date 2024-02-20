using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToolItemData", menuName = "New Item/Weapon", order = 3)]
public class WeaponItemData : ToolItemData
{
    public bool isTwoHandedTool;
    public bool isTwinTool;
    public int attackPower;
    public int damage;
}
