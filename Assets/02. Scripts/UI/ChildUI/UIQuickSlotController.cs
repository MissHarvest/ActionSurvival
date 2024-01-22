using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuickSlotController : UIBase
{
    private List<UIQuickSlot> _slots = new List<UIQuickSlot>();

    public override void Initialize()
    {
        
    }

    private void Awake()
    {
        var quickSlotSystem = Managers.Game.Player.QuickSlot;
        quickSlotSystem.OnUpdated += OnQuickSlotUpdated;
        CreateSlots(quickSlotSystem);
    }

    private void CreateSlots(QuickSlotSystem quickSlotSystem)
    {
        for (int i = 0; i < QuickSlotSystem.capacity; ++i)
        {
            var slotUIPrefab = Managers.Resource.GetCache<GameObject>("UIQuickSlot.prefab");
            var slotUI = Instantiate(slotUIPrefab, transform).GetOrAddComponent<UIQuickSlot>();
            slotUI.Init(this, i, quickSlotSystem.slots[i].itemSlot);
            _slots.Add(slotUI);
        }
    }

    private void OnQuickSlotUpdated(int index, ItemSlot itemSlot)
    {
        _slots[index].Set(itemSlot);
    }
}
