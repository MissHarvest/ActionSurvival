using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static int maxCapacity { get; } = 30;

    public ItemSlot[] slots;

    public event Action<int, ItemSlot> OnUpdated;

    private void Awake()
    {
        slots = new ItemSlot[maxCapacity];
        for(int i = 0; i < slots.Length; ++i)
        {
            slots[i] = new ItemSlot();
        }
    }

    public void AddItem(ItemSlot slot)
    {
        int emptyIndex = 0; // targetindex

        if(slot.itemData.stackable == false)
        {
            if(FindEmptyIndex(out emptyIndex))
            {
                slots[emptyIndex] = slot;
                OnUpdated?.Invoke(emptyIndex, slot);
                return;
            }
        }

        var itemSlot = FindItem(slot.itemData, out emptyIndex);
        if(itemSlot != null)
        {
            itemSlot.AddQuantity(slot.quantity);
            OnUpdated?.Invoke(emptyIndex, slot);
            return;
        }

        if(FindEmptyIndex(out emptyIndex))
        {
            slots[emptyIndex] = slot;
            OnUpdated?.Invoke(emptyIndex, slot);
        }
    }

    private bool FindEmptyIndex(out int index)
    {
        // FindItem 사용해서 구현할 수 있을지도
        for(int i = 0; i < slots.Length; ++i)
        {
            if (slots[i].itemData == null)
            {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }

    public ItemSlot FindItem(ItemData itemData, out int index)
    {
        for(int i = 0; i < slots.Length; ++i)
        {
            if (slots[i].itemData == itemData && slots[i].IsFull == false)
            {
                index = i;
                return slots[i];
            }    
        }
        index = -1;
        return null;
    }
}
