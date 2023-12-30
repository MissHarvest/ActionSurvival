using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotSystem : MonoBehaviour
{
    public static int capacity = 4;
    public ItemSlot[] slots = new ItemSlot[capacity];

    public event Action<int, ItemSlot> OnUpdated;

    private InventorySystem _inventory;

    private void Awake()
    {
        for(int i = 0; i < capacity; ++i)
        {
            slots[i] = new ItemSlot();
        }
        _inventory = Managers.Game.Player.Inventory;
    }

    public void Regist(int slotIndex, int sourceIndex)
    {
        UnRegist(slotIndex);

        slots[slotIndex] = _inventory.slots[sourceIndex];
        slots[slotIndex].bUse = true;
        OnUpdated?.Invoke(slotIndex, slots[slotIndex]);
    }

    public void UnRegist(int index)
    {
        if (slots[index].itemData != null)
        {
            slots[index].bUse = false;
        }
    }

    public void Use(int index) // Change , Consume
    {

    }
}
