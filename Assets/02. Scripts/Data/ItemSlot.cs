using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    public ItemData itemData;
    [field: SerializeField] public int quantity { get; private set; }
    // 내구도 
    public bool bUse;

    public ItemSlot()
    {
        this.itemData = null;
        this.quantity = 0;
    }

    public ItemSlot(ItemData itemData)
    {
        this.itemData = itemData;
        this.quantity = 1;
    }

    public ItemSlot(ItemData itemData, int quantity)
    {
        this.itemData = itemData;
        this.quantity = quantity;
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
            this.itemData = null;
        }
        // To Do ) 소모해야하는 양 보다 가지고 있는게 적으면 실패하는 로직
    }

    public void Clear()
    {
        itemData = null;
        quantity = 0;
    }
}
