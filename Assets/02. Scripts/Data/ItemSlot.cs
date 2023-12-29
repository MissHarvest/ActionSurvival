using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    public ItemData itemData;
    [field: SerializeField] public int quantity { get; private set; }
    // ������ 
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

    public void SubtractQuantity(int amount)
    {
        // �Ҹ��ؾ��ϴ� �� ���� ������ �ִ°� ������ �����ϴ� ����
    }

    public void Clear()
    {
        itemData = null;
        quantity = 0;
    }
}
