using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
//using static UnityEditor.Progress;

[Serializable]
public class ItemSlot
{
    public ItemData itemData { get; private set; } = null;
    [field: SerializeField] public string itemName { get; private set; } = string.Empty;
    [field: SerializeField] public int quantity { get; private set; }
    
    // 내구도 굳이 float 이어야하나?
    [field: SerializeField] public float currentDurability { get; private set; }

    [field: SerializeField] public bool equipped { get; private set; } = false;
    [field: SerializeField] public bool registed { get; private set; } = false;

    public InventorySystem inventory { get; set; }

    /// <summary>
    /// [사용하지 않는 생성자]
    /// </summary>
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
        this.currentDurability = (itemData is EquipItemData toolItem) ? toolItem.MaxDurability : 0f;
    }

    public bool IsFull => this.quantity == itemData.MaxStackCount;

    public int AddQuantity(int amount)
    {
        this.quantity += amount;
        Debug.Log($"[Max Stack Count] {itemData.MaxStackCount}");
        var remain = this.quantity > itemData.MaxStackCount ? this.quantity - itemData.MaxStackCount : 0;
        this.quantity -= remain;
        return remain;
    }

    public int SubtractQuantity(int amount = 1)
    {
        this.quantity -= amount;
        var over = this.quantity < 0 ? -1 * this.quantity : 0;
        this.quantity += over;
        if(quantity == 0)
        {
            Clear();
        }
        return over;
    }

    public void LoadData()
    {
        if (itemName == string.Empty) return;
        var path = $"{itemName}.data";
        Debug.Log($"[{path}] Finding");
        itemData = Managers.Resource.GetCache<ItemData>(path);
    }

    public void Set(ItemData item)
    {
        this.itemData = item;
        this.itemName = item.name;
        this.quantity = 0;
        this.currentDurability = itemData.MaxDurability;
    }

    public void Set(ItemData itemData, float durability)
    {
        this.itemData = itemData;
        this.itemName = itemData.name;
        this.quantity = 0;
        this.currentDurability = durability == 0.0f ? itemData.MaxDurability : durability;
    }

    public void Set(ItemData item, int quantity)
    {
        // [ Check ] // 내구도 부분이랑 병합 시 
        this.itemData = item;
        this.itemName = item.name;
        this.quantity = quantity;
        this.currentDurability = (itemData is EquipItemData toolItem) ? toolItem.MaxDurability : 0f;
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
        this.currentDurability = Mathf.Clamp(value, 0f, (itemData is EquipItemData toolItem) ? toolItem.MaxDurability : 0f);
        if (currentDurability <= 0)
            Clear();
    }

    public void SubtractDurability(float amount)
    {
        if (itemData.MaxDurability == 0.0f) return;
        this.currentDurability -= amount;
        if (this.currentDurability <= 0.0f)
            Clear();
    }
}
