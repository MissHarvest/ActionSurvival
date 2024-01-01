using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuickSlot : UIItemSlot
{
    public UIQuickSlotController UIQuickSlotController { get; private set; }
    public int index { get; private set; }

    public void Init(UIQuickSlotController quickSlotControllerUI, int index, ItemSlot itemSlot)
    {
        UIQuickSlotController = quickSlotControllerUI;
        this.index = index;
        Set(itemSlot);
    }

    private void Awake()
    {
        Initialize();

        gameObject.BindEvent((x) =>
        {
            Managers.Game.Player.QuickSlot.OnQuickUseInput(index);
        });
    }

    public override void Set(ItemSlot itemSlot)
    {
        if(itemSlot == null)
        {
            Clear();
            return;
        }

        base.Set(itemSlot);
    }
}
