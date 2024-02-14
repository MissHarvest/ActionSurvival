using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
//using static UnityEditor.Progress;

[Serializable]
public class ItemSlot
{
    public ItemData itemData { get; private set; } = null;
    [field: SerializeField] public string itemName { get; private set; } = string.Empty;
    [field: SerializeField] public int quantity { get; private set; }
    // 내구도 
    [field: SerializeField] public float currentDurability { get; private set; }

    [field: SerializeField] public bool equipped { get; private set; } = false;
    [field: SerializeField] public bool registed { get; private set; } = false;

    public InventorySystem inventory { get; set; }

    public ItemSlot()
    {
        this.itemData = null;
        this.itemName = string.Empty;
        this.quantity = 0;
        this.currentDurability = 0.0f;
    }

    public ItemSlot(InventorySystem inventory)
    {
        this.inventory = inventory;
        this.itemName = string.Empty;
        this.itemData = null;
        this.quantity = 0;
        this.currentDurability = 0.0f;
    }

    public ItemSlot(ItemData itemData, int quantity = 1)
    {
        this.itemData = itemData;
        this.itemName = itemData.name;
        this.quantity = quantity;
        this.currentDurability = (itemData is EquipItemData toolItem) ? toolItem.maxDurability : 0f;
    }

    public bool IsFull => this.quantity == ItemData.maxStackCount;

    public void AddQuantity(int amount)
    {
        this.quantity += amount;
        if(this.quantity > ItemData.maxStackCount)
        {
            int extra = this.quantity - ItemData.maxStackCount;
            this.quantity = ItemData.maxStackCount;
            inventory.AddItem(itemData, extra);
        }
    }

    public void SubtractQuantity(int amount = 1)
    {
        this.quantity = Math.Max(this.quantity - amount, 0);
        if(quantity == 0)
        {
            Clear();
        }
        // To Do ) 소모해야하는 양 보다 가지고 있는게 적으면 실패하는 로직
    }

    public void LoadData()
    {
        if (itemName == string.Empty) return;
        var path = $"{itemName}.data";
        Debug.Log($"[{path}] Finding");
        itemData = Managers.Resource.GetCache<ItemData>(path);
    }

    public void Set(ItemData item, int quantity = 1)
    {
        // [ Check ] // 내구도 부분이랑 병합 시 
        this.itemData = item;
        this.itemName = item.name;
        this.quantity = quantity;
        this.currentDurability = (itemData is EquipItemData toolItem) ? toolItem.maxDurability : 0f;
    }

    public void Set(ItemSlot itemSlot)
    {
        // [ Check ] // 내구도 부분이랑 병합 시 
        Set(itemSlot.itemData, itemSlot.quantity);
        registed = itemSlot.registed;
        equipped = itemSlot.equipped;
        currentDurability = itemSlot.currentDurability;
    }

    public void Copy(ItemSlot itemslot)
    {
        Set(itemslot.itemData, itemslot.quantity);
        registed = itemslot.registed;
        equipped = itemslot.equipped;
        currentDurability = itemslot.currentDurability;
        inventory = itemslot.inventory;
    }

    public void Clear()
    {
        itemData = null;
        itemName = string.Empty;
        quantity = 0;
        registed = false;
        equipped = false;
        currentDurability = 0.0f;
    }

    public void SetRegist(bool value)
    {
        this.registed = value;
    }

    public void SetEquip(bool value)
    {
        this.equipped = value;
    }

    public void SetDurability(float value)
    {
        this.currentDurability = Mathf.Clamp(value, 0f, (itemData is EquipItemData toolItem) ? toolItem.maxDurability : 0f);
        if (currentDurability <= 0)
            Clear();
    }
}
