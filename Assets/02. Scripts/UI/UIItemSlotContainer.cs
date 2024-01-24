using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

public class UIItemSlotContainer : UIBase
{
    private List<UIItemSlot> _uiItemSlots = new List<UIItemSlot>();

    public override void Initialize()
    {
        
    }

    private void Start()
    {
        
    }

    public virtual void CreateItemSlots<T>(GameObject slotPrefab, int count) where T : UIItemSlot
    {
        for (int i = 0; i < count; ++i)
        {
            var itemSlotUI = Instantiate(slotPrefab, this.transform);
            var inventoryslotUI = itemSlotUI.GetComponent<T>();
            _uiItemSlots.Add(inventoryslotUI);
        }
    }

    public virtual void Init<T>(InventorySystem inventory, Action<UIItemSlot> action) where T : UIItemSlot
    {
        for(int i = 0; i < _uiItemSlots.Count; ++i)
        {
            var slot = _uiItemSlots[i] as T;
            if (slot != null)
            {
                slot.BindGroup(this, i);
                slot.Set(inventory.slots[i]);
                slot.OnClicked(action);
            }
        }
    }

    public void OnItemSlotUpdated(int index, ItemSlot itemSlot)
    {
        _uiItemSlots[index].Set(itemSlot);
    }
}
