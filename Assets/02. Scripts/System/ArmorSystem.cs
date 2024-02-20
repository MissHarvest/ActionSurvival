using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
// Lee gyuseong 24.02.07

public class ArmorSystem : MonoBehaviour
{
    [SerializeField] private int _defense;

    public QuickSlot[] equippedArmors;

    public event Action<QuickSlot> OnEquipArmor;
    public event Action<QuickSlot> OnUnEquipArmor;

    private void Awake()
    {
        equippedArmors = new QuickSlot[2];
        for (int i = 0; i < equippedArmors.Length; i++)
        {
            equippedArmors[i] = new QuickSlot();
        }
        //Managers.Game.Player.OnHit += OnUpdateDurabilityOfArmor;

        Load();

        Managers.Game.OnSaveCallback += Save;
    }

    public void Equip(QuickSlot quickSlot)
    {
        int part = GetPart(quickSlot);

        AddDefenseOfArmor(quickSlot);

        equippedArmors[part].Set(quickSlot.targetIndex, quickSlot.itemSlot);
        equippedArmors[part].itemSlot.SetEquip(true);

        OnEquipArmor?.Invoke(equippedArmors[part]);
    }

    public void UnEquip(QuickSlot quickSlot)
    {
        int part = GetPart(quickSlot);

        SubtractDefenseOfArmor(quickSlot);

        equippedArmors[part].itemSlot.SetEquip(false);
        OnUnEquipArmor?.Invoke(equippedArmors[part]);

        equippedArmors[part].Clear();
    }

    private void AddDefenseOfArmor(QuickSlot quickSlot)
    {
        EquipItemData toolItemData = GetArmorItemData(quickSlot);
        _defense += toolItemData.defense;
        Managers.Game.Player.playerDefense = _defense;
    }

    private void SubtractDefenseOfArmor(QuickSlot quickSlot)
    {
        EquipItemData toolItemData = GetArmorItemData(quickSlot);
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

    private EquipItemData GetArmorItemData(QuickSlot quickSlot)
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
            for (int i = 0; i < equippedArmors.Length; ++i)
            {
                equippedArmors[i].itemSlot.LoadData();
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
