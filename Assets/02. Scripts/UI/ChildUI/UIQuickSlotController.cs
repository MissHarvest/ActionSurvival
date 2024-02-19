using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        _slots = GetComponentsInChildren<UIQuickSlot>().ToList();

        for (int i = 0; i < _slots.Count; ++i)
        {
            _slots[i].Init(this, i, quickSlotSystem.slots[i].itemSlot);
        }
    }

    private void OnQuickSlotUpdated(int index, ItemSlot_Class itemSlot)
    {
        _slots[index].Set(itemSlot);
    }
}
