using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Lee gyuseong 24.02.07

public class ArmorSystem : MonoBehaviour
{
    private int _defense;
    private float _maxDurability;
    [SerializeField] private List<QuickSlot> _linkedSlots = new List<QuickSlot>();
    public Condition HP { get; private set; }

    private void Awake()
    {
        Managers.Game.Player.ToolSystem.OnEquip += DefenseOfTheEquippedArmor;
        Managers.Game.Player.ToolSystem.OnUnEquip += DefenseOfTheUnEquippedArmor;
        Managers.Game.Player.onHit += Duration;        
    }

    public void DefenseOfTheEquippedArmor(QuickSlot quickSlot)
    {
        ItemData armor = quickSlot.itemSlot.itemData;
        EquipItemData toolItemDate = (EquipItemData)armor;
        _defense += toolItemDate.defense;
        Managers.Game.Player.playerDefense = _defense;

        _linkedSlots.Add(quickSlot);
    }

    public void DefenseOfTheUnEquippedArmor(QuickSlot quickSlot)
    {
        ItemData armor = quickSlot.itemSlot.itemData;
        EquipItemData toolItemDate = (EquipItemData)armor;
        _defense -= toolItemDate.defense;
        Managers.Game.Player.playerDefense = _defense;

        _linkedSlots.Remove(quickSlot);
    }

    public void Duration() // Player Hit에서 호출
    {
        foreach (var items in _linkedSlots)
        {
            Managers.Game.Player.Inventory.UseToolItemByIndex(items.targetIndex, 10);
        }
    }
}
