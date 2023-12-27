using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : UIPopup
{
    private Transform _contents;
    private List<UIItemSlot> _uiItemSlots = new List<UIItemSlot>();

    private void Awake()
    {
        _contents = FindItemSlotRoot(this.transform);

        // Create Slot
        var inventory = GameObject.Find("Player").GetComponentInChildren<InventorySystem>();
        inventory.OnUpdated += OnItemSlotUpdated;
        CreateItemSlots(InventorySystem.maxCapacity);
        gameObject.SetActive(false);
    }

    private void CreateItemSlots(int capacity)
    {
        var itemSlotPrefab = Resources.Load<GameObject>("UI/UI_ItemSlot");
        for(int i = 0; i < capacity; ++i)
        {
            var itemSlotUI = Instantiate(itemSlotPrefab, _contents.transform);
            var uiItemSlot = itemSlotUI.GetComponent<UIItemSlot>();
            uiItemSlot?.SetIndex(i);
            _uiItemSlots.Add(uiItemSlot);
        }
    }

    private Transform FindItemSlotRoot(Transform root)
    {
        for(int i = 0; i < root.childCount; ++i)
        {
            if(root.GetChild(i).childCount != 0)
            {
                return FindItemSlotRoot(root.GetChild(i));
            }
            else
            {
                if(root.GetChild(i).name == "Contents")
                {
                    return root.GetChild(i);
                }
            }
        }
        return null;
    }

    private void OnItemSlotUpdated(int index, ItemSlot itemSlot)
    {
        _uiItemSlots[index].Set(itemSlot);
    }
}
