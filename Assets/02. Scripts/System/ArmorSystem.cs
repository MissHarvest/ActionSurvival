using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;
// Lee gyuseong 24.02.07

public class ArmorSystem : MonoBehaviour
{
    public int _defense;

    public QuickSlot[] _linkedSlots;

    public event Action<QuickSlot> EquipArmor;
    public event Action<QuickSlot> UnEquipArmor;

    private void Awake()
    {
        _linkedSlots = new QuickSlot[2];

        Managers.Game.Player.ToolSystem.OnEquip += DefenseOfTheEquippedArmor;
        Managers.Game.Player.ToolSystem.OnUnEquip += DefenseOfTheUnEquippedArmor;
        Managers.Game.Player.OnHit += Duration;

        Load();

        Managers.Game.OnSaveCallback += Save;
    }

    private void Start()
    {
        Managers.Game.Player.Inventory.OnUpdated += OnInventoryUpdated;
    }

    public void DefenseOfTheEquippedArmor(QuickSlot quickSlot)
    {
        EquipItemData toolItemData = ArmorDefense(quickSlot);
        _defense += toolItemData.defense;
        Managers.Game.Player.playerDefense = _defense;

        if ((int)toolItemData.part == 0 || (int)toolItemData.part == 1)
        {
            _linkedSlots[(int)toolItemData.part] = quickSlot;
            EquipArmor?.Invoke(quickSlot);
        }
    }

    public void DefenseOfTheUnEquippedArmor(QuickSlot quickSlot)
    {
        EquipItemData toolItemData = ArmorDefense(quickSlot);
        _defense -= toolItemData.defense;
        Managers.Game.Player.playerDefense = _defense;

        if ((int)toolItemData.part == 0 || (int)toolItemData.part == 1)
        {
            _linkedSlots[(int)toolItemData.part] = null;
            UnEquipArmor?.Invoke(quickSlot);
        }
    }

    public void Duration() // Player Hit에서 호출
    {
        for (int i = 0; i < _linkedSlots.Length; i++)
        {
            if (_linkedSlots[i] != null && _linkedSlots[i].targetIndex != -1)
            {
                Managers.Game.Player.Inventory.UseToolItemByIndex(_linkedSlots[i].targetIndex, 1);
            }
        }
    }

    public void OnInventoryUpdated(int inventoryIndex, ItemSlot itemSlot)
    {
        for (int i = 0; i < 2; i++)
        {
            if (_linkedSlots[i] != null)
            {
                if (_linkedSlots[i].targetIndex == inventoryIndex)
                {
                    if (itemSlot.itemData == null)
                    {
                        EquipItemData toolItemData = ArmorDefense(_linkedSlots[i]);
                        _defense -= toolItemData.defense;
                        Managers.Game.Player.playerDefense = _defense;

                        UnEquipArmor?.Invoke(_linkedSlots[i]);
                        _linkedSlots[i].Clear();
                        Managers.Game.Player.ToolSystem.UnEquipArmor(_linkedSlots[i]);
                    }
                }
            }            
        }
    }

    private EquipItemData ArmorDefense(QuickSlot quickSlot)
    {
        ItemData armor = quickSlot.itemSlot.itemData;
        EquipItemData toolItemDate = (EquipItemData)armor;
        return toolItemDate;
    }

    private void UpdatePlayerDefense()
    {
        Managers.Game.Player.playerDefense = _defense;
    }

    private void Load()
    {
        if (SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "ArmorSystem"))
        {
            for (int i = 0; i < _linkedSlots.Length; ++i)
            {
                _linkedSlots[i].itemSlot.LoadData();
            }
            UpdatePlayerDefense();
        }
    }

    private void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile("ArmorSystem", json, SaveGame.SaveType.Runtime);
    }
}
