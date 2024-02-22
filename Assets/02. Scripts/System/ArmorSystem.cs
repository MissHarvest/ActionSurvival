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

    [SerializeField] private QuickSlot[] _equippedArmors;

    public event Action<QuickSlot> OnEquipArmor;
    public event Action<QuickSlot> OnUnEquipArmor;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();

        _equippedArmors = new QuickSlot[2];
        for (int i = 0; i < _equippedArmors.Length; i++)
        {
            _equippedArmors[i] = new QuickSlot();
        }

        GameManager.Instance.Player.OnHit += OnUpdateDurabilityOfArmor;

        Load();

        GameManager.Instance.OnSaveCallback += Save;
    }

    private void Start()
    {
        GameManager.Instance.Player.Inventory.OnUpdated += OnInventoryUpdated;
    }

    public void Equip(int index, ItemSlot itemSlot)
    {
        var part = GetPart(itemSlot);

        UnEquip(part);

        itemSlot.SetEquip(true);
        _equippedArmors[(int)part].Set(index, itemSlot);

        AddDefenseOfArmor(_equippedArmors[(int)part]);

        OnEquipArmor?.Invoke(_equippedArmors[(int)part]);
    }

    public void UnEquip(ItemParts part)
    {
        if (_equippedArmors[(int)part].itemSlot.itemData == null) return;

        SubtractDefenseOfArmor(_equippedArmors[(int)part]);

        _equippedArmors[(int)part].itemSlot.SetEquip(false);
        OnUnEquipArmor?.Invoke(_equippedArmors[(int)part]);

        _equippedArmors[(int)part].Clear();
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
        for (int i = 0; i < _equippedArmors.Length; i++)
        {
            if (_equippedArmors[i] != null && _equippedArmors[i].targetIndex != -1)
            {
                _player.Inventory.TrySubtractDurability(_equippedArmors[i].targetIndex, 1);
            }
        }
    }

    private EquipItemData GetArmorItemData(QuickSlot quickSlot)
    {
        EquipItemData toolItemDate = quickSlot.itemSlot.itemData as EquipItemData;
        return toolItemDate;
    }

    private ItemParts GetPart(ItemSlot slot)
    {
        var itemData = slot.itemData as EquipItemData;
        return itemData.part;
    }

    public int GetDefense()
    {
        return _defense;
    }

    public QuickSlot[] GetEquippedArmorsArray()
    {
        return _equippedArmors;
    }

    public void OnInventoryUpdated(int inventoryIndex, ItemSlot itemSlot)
    {
        if (itemSlot.itemData != null) return;
        for (int i = 0; i < _equippedArmors.Length; i++)
        {
            if (_equippedArmors[i] != null)
            {
                if (_equippedArmors[i].targetIndex == inventoryIndex && itemSlot.itemData == null)
                {
                    SubtractDefenseOfArmor(_equippedArmors[i]);
                    OnUnEquipArmor?.Invoke(_equippedArmors[i]);
                    _equippedArmors[i].Clear();
                }
            }
        }
    }

    private void Load()
    {
        if (SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "ArmorSystem"))
        {
            for (int i = 0; i < _equippedArmors.Length; ++i)
            {
                _equippedArmors[i].itemSlot.LoadData();
            }
        }
    }

    private void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile("ArmorSystem", json, SaveGame.SaveType.Runtime);
    }
}
