using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeFire : MonoBehaviour
{
    //화로가 가지게
    public ItemSlot[] firewoodItemSlots = new ItemSlot[2];

    public int _count = 0;
    public int _maxCount;
    public int _index;

    public event Action<int> OnUpdatedCount;
    public event Action OnUpdatedUI;

    private void Awake()
    {
        GetFirewoodItems();
    }

    private void GetFirewoodItems()
    {
        var itemData = Managers.Resource.GetCache<ItemData>("BranchItemData.data");
        firewoodItemSlots[0].Set(itemData, 0);
        itemData = Managers.Resource.GetCache<ItemData>("LogItemData.data");
        firewoodItemSlots[1].Set(itemData, 0);
    }

    public void OnTakeoutQuantity()
    {
        var itemSlots = firewoodItemSlots[_index];

        if (itemSlots.quantity > 0)
        {
            GameManager.Instance.Player.Inventory.TryAddItem(itemSlots.itemData, itemSlots.quantity);
            firewoodItemSlots[_index].SubtractFirewoodItemQuantity(itemSlots.quantity);

            OnUpdatedUI?.Invoke();
        }
    }

    public void OnStoreQuantity()
    {
        if (_count > 0)
        {
            firewoodItemSlots[_index].AddQuantity(_count);
            GameManager.Instance.Player.Inventory.TryConsumeQuantity(firewoodItemSlots[_index].itemData, _count);

            OnUpdatedUI?.Invoke();
        }
    }

    public void OnMinusQuantity()
    {
        if (_count > 0)
        {
            _count--;
            UpdateCountUI();
        }
    }

    public void OnPlusQuantity()
    {
        if (_count < _maxCount)
        {
            _count++;
            UpdateCountUI();
        }
    }

    private void UpdateCountUI()
    {
        OnUpdatedCount?.Invoke(_count);
    }
}
