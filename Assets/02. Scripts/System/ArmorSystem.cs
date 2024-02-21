using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
// Lee gyuseong 24.02.07

public class ArmorSystem : MonoBehaviour
{
    private Player _player;
    [SerializeField] private int _defense;
    private int _indexInUse = -1;

    public QuickSlot[] equippedArmors;

    public event Action<QuickSlot> OnEquipArmor;
    public event Action<QuickSlot> OnUnEquipArmor;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();

        equippedArmors = new QuickSlot[2];
        for (int i = 0; i < equippedArmors.Length; i++)
        {
            equippedArmors[i] = new QuickSlot();
        }
        Managers.Game.Player.OnHit += OnUpdateDurabilityOfArmor;

        Load();

        Managers.Game.OnSaveCallback += Save;
    }

    public void Equip(int index, ItemSlot itemSlot)
    {
        int part = GetPart(itemSlot);

        itemSlot.SetEquip(true);
        equippedArmors[part].Set(index, itemSlot);

        AddDefenseOfArmor(equippedArmors[part]);

        OnEquipArmor?.Invoke(equippedArmors[part]);
    }

    public void UnEquip(int index)
    {
        for (int i = 0; i < equippedArmors.Length; i++)
        {
            if (equippedArmors[i].itemSlot.itemData != null && equippedArmors[i].targetIndex == index)
            {
                SubtractDefenseOfArmor(equippedArmors[i]);

                equippedArmors[i].itemSlot.SetEquip(false);
                OnUnEquipArmor?.Invoke(equippedArmors[i]);

                equippedArmors[i].Clear();
                return;
            }
        }
    }

    private void AddDefenseOfArmor(QuickSlot quickSlot)
    {
        EquipItemData toolItemData = GetArmorItemData(quickSlot);
        _defense += toolItemData.defense;
    }

    private void SubtractDefenseOfArmor(QuickSlot quickSlot)
    {
        EquipItemData toolItemData = GetArmorItemData(quickSlot);
        _defense -= toolItemData.defense;
    }

    public void OnUpdateDurabilityOfArmor()
    {
        for (int i = 0; i < equippedArmors.Length; i++)
        {
            if (equippedArmors[i] != null && equippedArmors[i].targetIndex != -1)
            {
                _player.Inventory.TrySubtractDurability(equippedArmors[i].targetIndex, 1);
            }
        }
    }

    private EquipItemData GetArmorItemData(QuickSlot quickSlot)
    {
        EquipItemData toolItemDate = quickSlot.itemSlot.itemData as EquipItemData;
        return toolItemDate;
    }

    private int GetPart(ItemSlot slot)
    {
        var itemData = slot.itemData as EquipItemData;
        if (itemData == null) return -1;
        return (int)itemData.part;
    }

    public int GetDefense() 
    { 
        return _defense;
    }

    private void Load()
    {
        if (SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "ArmorSystem"))
        {
            for (int i = 0; i < equippedArmors.Length; ++i)
            {
                equippedArmors[i].itemSlot.LoadData();
            }
        }
    }

    private void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile("ArmorSystem", json, SaveGame.SaveType.Runtime);
    }
}
