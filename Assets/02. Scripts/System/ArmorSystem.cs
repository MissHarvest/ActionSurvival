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

        GameManager.Instance.OnSaveCallback += Save;
    }

    private void Start()
    {
        Managers.Game.Player.Inventory.OnUpdated += OnInventoryUpdated;
    }

    public void Equip(int index, ItemSlot itemSlot)
    {
        int part = GetPart(itemSlot);

        TakeOffEquippedArmor(part);

        itemSlot.SetEquip(true);
        equippedArmors[part].Set(index, itemSlot);

        AddDefenseOfArmor(equippedArmors[part]);

        OnEquipArmor?.Invoke(equippedArmors[part]);
    }

    public void UnEquip(int index)//enum parts를 변수로 받아서
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

    private void TakeOffEquippedArmor(int index)//enum
    {
        if (equippedArmors[index].itemSlot.itemData == null) return;

        SubtractDefenseOfArmor(equippedArmors[index]);

        equippedArmors[index].itemSlot.SetEquip(false);
        OnUnEquipArmor?.Invoke(equippedArmors[index]);

        equippedArmors[index].Clear();
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

    public void OnInventoryUpdated(int inventoryIndex, ItemSlot itemSlot)
    {
        for (int i = 0; i < equippedArmors.Length; i++)
        {
            if (equippedArmors[i] != null)
            {
                if (equippedArmors[i].targetIndex == inventoryIndex)
                {
                    if (itemSlot.itemData == null)
                    {
                        EquipItemData toolItemData = GetArmorItemData(equippedArmors[i]);
                        _defense -= toolItemData.defense;

                        OnUnEquipArmor?.Invoke(equippedArmors[i]);
                        equippedArmors[i].Clear();
                    }
                }
            }
        }
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
