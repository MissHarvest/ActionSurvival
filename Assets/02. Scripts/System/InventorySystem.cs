using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private static int _maxCapacity = 10;

    public ItemSlot[] slots;

    private void Awake()
    {
        slots = new ItemSlot[_maxCapacity];
        for(int i = 0; i < slots.Length; ++i)
        {
            slots[i] = new ItemSlot();
        }
    }

    public void AddItem(ItemSlot slot)
    {
        int emptyIndex = 0;

        if(slot.itemData.stackable == false)
        {
            if(FindEmptyIndex(out emptyIndex))
            {
                slots[emptyIndex] = slot;
                return;
            }
        }

        var itemSlot = FindItem(slot.itemData);
        if(itemSlot != null)
        {
            itemSlot.AddQuantity(slot.quantity);
            return;
        }

        if(FindEmptyIndex(out emptyIndex))
        {
            slots[emptyIndex] = slot;
        }
    }

    private bool FindEmptyIndex(out int index)
    {
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

    public ItemSlot FindItem(ItemData itemData)
    {
        for(int i = 0; i < slots.Length; ++i)
        {
            if (slots[i].itemData == itemData && slots[i].IsFull == false)
            {
                return slots[i];
            }    
        }
        return null;
    }
}
