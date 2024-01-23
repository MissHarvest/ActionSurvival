using System;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    [field: SerializeField] public ItemData itemData { get; private set; } = null;
    [field: SerializeField] public int quantity { get; private set; }
    // 내구도 
    [field: SerializeField] public float currentDurability { get; private set; }

    public bool equipped { get; private set; } = false;
    public bool registed { get; private set; } = false;

    public ItemSlot()
    {
        this.itemData = null;
        this.quantity = 0;
        this.currentDurability = 0.0f;
    }

    public ItemSlot(ItemData itemData, int quantity = 1)
    {
        this.itemData = itemData;
        this.quantity = quantity;
        this.currentDurability = (itemData is ToolItemData toolItem) ? toolItem.maxDurability : 0f;
    }

    public bool IsFull => this.quantity == ItemData.maxStackCount;

    public void AddQuantity(int amount)
    {
        this.quantity = Math.Min(this.quantity + amount, ItemData.maxStackCount);        
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

    public void Set(ItemData item, int quantity = 1)
    {
        // [ Check ] // 내구도 부분이랑 병합 시 
        this.itemData = item;
        this.quantity = quantity;
        this.currentDurability = (itemData is ToolItemData toolItem) ? toolItem.maxDurability : 0f;
    }

    public void Set(ItemSlot itemSlot)
    {
        // [ Check ] // 내구도 부분이랑 병합 시 
        Set(itemSlot.itemData, itemSlot.quantity);
        registed = itemSlot.registed;
        equipped = itemSlot.equipped;
        currentDurability = itemSlot.currentDurability;
    }

    public void Clear()
    {
        itemData = null;
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
        this.currentDurability = Mathf.Clamp(value, 0f, (itemData is ToolItemData toolItem) ? toolItem.maxDurability : 0f);
        if (currentDurability <= 0)
            Clear();
    }
}
