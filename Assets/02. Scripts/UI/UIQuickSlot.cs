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
            // QuickSlotSystem. �� index. ��. ��� / ���� / �� �߿� �����ΰ�
            // Managers.Game.Player.ToolSystem.
        });
    }

    public override void Set(ItemSlot itemSlot)
    {
        if(itemSlot.bUse == false)
        {
            Clear();
            return;
        }

        base.Set(itemSlot);
    }
}
