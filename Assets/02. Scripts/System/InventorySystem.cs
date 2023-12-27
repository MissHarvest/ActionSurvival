using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySystem : MonoBehaviour
{
    public static int maxCapacity { get; } = 30;

    public ItemSlot[] slots;

    public event Action<int, ItemSlot> OnUpdated;

    private UIInventory _inventoryUI;

    private void Awake()
    {
        slots = new ItemSlot[maxCapacity];
        for(int i = 0; i < slots.Length; ++i)
        {
            slots[i] = new ItemSlot();
        }

        var input = GetComponentInParent<PlayerInput>();
        input.InputActions.Player.Inventory.started += OnInventoryShowAndHide;
    }

    public void AddItem(ItemSlot slot)
    {
        int targetindex = 0;

        if(slot.itemData.stackable == false)
        {
            if(FindEmptyIndex(out targetindex))
            {
                slots[targetindex] = slot;
                OnUpdated?.Invoke(targetindex, slot);
                return;
            }
        }

        var itemSlot = FindItem(slot.itemData, out targetindex);
        if(itemSlot != null)
        {
            itemSlot.AddQuantity(slot.quantity);
            OnUpdated?.Invoke(targetindex, slot);
            return;
        }

        if(FindEmptyIndex(out targetindex))
        {
            slots[targetindex] = slot;
            OnUpdated?.Invoke(targetindex, slot);
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

    private void OnInventoryShowAndHide(InputAction.CallbackContext context)
    {
        if (_inventoryUI == null)
        {
            _inventoryUI = Managers.UI.ShowPopupUI<UIInventory>();
        }
        else
        {
            Managers.UI.ClosePopupUI(_inventoryUI);
        }        
    }
}
