using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotSystem : MonoBehaviour
{
    public static int capacity = 4;
    public QuickSlot[] slots  = new QuickSlot[capacity];

    public event Action<int, ItemSlot> OnUpdated;

    private InventorySystem _inventory;

    private void Awake()
    {
        for(int i = 0; i < capacity; ++i)
        {
            slots[i] = new QuickSlot();
        }
        _inventory = Managers.Game.Player.Inventory;
    }

    public void Regist(int index, int target)
    {
        var itemSlot = _inventory.RegistItemByIndex(target);
        slots[index].Set(target, itemSlot);
        OnUpdated?.Invoke(index, itemSlot);
    }

    public void UnRegist(int target)
    {
        var itemSlot = _inventory.UnRegistItemByIndex(target);

        for(int i = 0; i <  slots.Length; ++i)
        {
            if (target == slots[i].targetIndex)
            {
                slots[i].itemSlot = null;
                OnUpdated?.Invoke(i, itemSlot);
            }
        }

        // 손에 잡고 있는 오브젝트 였으면. 삭제
    }

    public void Use(int index) // Change , Consume
    {

    }
}
