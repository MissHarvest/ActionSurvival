using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItemData", menuName = "New Item/Weapon", order = 0)]
public class WeaponItemData : ItemData
{
    public LayerMask targetLayers;
    public float range;
    public int attackPower;
    public string tagName;

    public WeaponItemData()
    {
        stackable = false;
        registable = true;
    }
}
