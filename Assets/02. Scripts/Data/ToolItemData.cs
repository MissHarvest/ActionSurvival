using UnityEngine;

[CreateAssetMenu(fileName = "ToolItemData", menuName = "New Item/Weapon", order = 0)]
public class ToolItemData : EquipItemData
{
    public LayerMask targetLayers;
    public float range;
    public int attackPower;
    public int damage;
    public string targetTagName;

    public bool isWeapon;
    public bool isTwoHandedTool;
    public bool isTwinTool;

    public bool isArchitecture;

    public ToolItemData()
    {
        //stackable = false;
        registable = true;
        part = ItemParts.Hand;

        isTwoHandedTool = false;
    }
}
