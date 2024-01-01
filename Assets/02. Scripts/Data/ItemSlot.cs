using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class ItemSlot
{
    public ItemData itemData;
    [field: SerializeField] public int quantity { get; private set; }
    // ������ 

    public bool equipped { get; private set; } = false;
    public bool registed { get; private set; } = false;

    public ItemSlot()
    {
        this.itemData = null;
        this.quantity = 0;
    }

    public ItemSlot(ItemData itemData, int quantity = 1)
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
        // To Do ) �Ҹ��ؾ��ϴ� �� ���� ������ �ִ°� ������ �����ϴ� ����
    }

    public void Clear()
    {
        itemData = null;
        quantity = 0;
        registed = false;
        equipped = false;
    }

    public void SetRegist(bool value)
    {
        this.registed = value;
    }

    public void SetEquip(bool value)
    {
        this.equipped = value;
    }
}
