using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
// Lee gyuseong 24.02.07

public class ArmorSystem : MonoBehaviour
{
    //ToolSystem은 Handable 아이템만 관리 방어구는 여기서 다 관리
    public int _defense;

    public QuickSlot[] _equippedArmor;

    public event Action<QuickSlot> OnEquipArmor;
    public event Action<QuickSlot> OnUnEquipArmor;

    private void Awake()
    {
        _equippedArmor = new QuickSlot[2];
        for (int i = 0; i < _equippedArmor.Length; i++)
        {
            _equippedArmor[i] = new QuickSlot();
        }

        Load();

        Managers.Game.OnSaveCallback += Save;
    }

    private void Start()
    {
        Managers.Game.Player.Inventory.OnUpdated += OnInventoryUpdated;
    }

    public void Equip(QuickSlot quickSlot)
    {
        int part = GetPart(quickSlot);

        AddDefenseOfArmor(quickSlot);

        _equippedArmor[part].Set(quickSlot.targetIndex, quickSlot.itemSlot);
        _equippedArmor[part].itemSlot.SetEquip(true);

        OnEquipArmor?.Invoke(_equippedArmor[part]);
    }

    public void UnEquip(QuickSlot quickSlot)
    {
        int part = GetPart(quickSlot);

        SubtractDefenseOfArmor(quickSlot);

        _equippedArmor[part].itemSlot.SetEquip(false);
        OnUnEquipArmor?.Invoke(_equippedArmor[part]);

        _equippedArmor[part].Clear();
    }

    public void AddDefenseOfArmor(QuickSlot quickSlot)
    {
        EquipItemData toolItemData = GetDefenseOfArmorItem(quickSlot);
        _defense += toolItemData.defense;
        Managers.Game.Player.playerDefense = _defense;
    }

    public void SubtractDefenseOfArmor(QuickSlot quickSlot)
    {
        EquipItemData toolItemData = GetDefenseOfArmorItem(quickSlot);
        _defense -= toolItemData.defense;
        Managers.Game.Player.playerDefense = _defense;
    }

    //public void OnUpdateDurabilityOfArmor() // Player Hit에서 호출
    //{
    //    for (int i = 0; i < _linkedSlots.Length; i++)
    //    {
    //        if (_linkedSlots[i] != null && _linkedSlots[i].targetIndex != -1)
    //        {
    //            Managers.Game.Player.Inventory.UseToolItemByIndex(_linkedSlots[i].targetIndex, 1);
    //        }
    //    }
    //}

    public void OnInventoryUpdated(int inventoryIndex, ItemSlot itemSlot)
    {
        for (int i = 0; i < 2; i++)
        {
            if (_equippedArmor[i] != null)
            {
                if (_equippedArmor[i].targetIndex == inventoryIndex)
                {
                    if (itemSlot.itemData == null)
                    {
                        EquipItemData toolItemData = GetDefenseOfArmorItem(_equippedArmor[i]);
                        _defense -= toolItemData.defense;
                        Managers.Game.Player.playerDefense = _defense;

                        OnUnEquipArmor?.Invoke(_equippedArmor[i]);
                        _equippedArmor[i].Clear();
                        Managers.Game.Player.ToolSystem.UnEquipArmor(_equippedArmor[i]);
                    }
                }
            }
        }
    }

    private EquipItemData GetDefenseOfArmorItem(QuickSlot quickSlot)
    {
        ItemData armor = quickSlot.itemSlot.itemData;
        EquipItemData toolItemDate = (EquipItemData)armor;
        return toolItemDate;
    }

    private void UpdatePlayerDefense()
    {
        Managers.Game.Player.playerDefense = _defense;
    }

    private int GetPart(QuickSlot slot)
    {
        var itemData = slot.itemSlot.itemData as EquipItemData;
        if (itemData == null) return -1;
        return (int)itemData.part;
    }

    private void Load()
    {
        if (SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "ArmorSystem"))
        {
            for (int i = 0; i < _equippedArmor.Length; ++i)
            {
                _equippedArmor[i].itemSlot.LoadData();
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
