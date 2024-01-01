using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickSlotSystem : MonoBehaviour
{
    public static int capacity = 4;
    public QuickSlot[] slots  = new QuickSlot[capacity];

    public event Action<int, ItemSlot> OnUpdated;
    public event Action<int> OnRegisted;
    public event Action<QuickSlot> OnUnRegisted;
        
    public int IndexInUse { get; private set; } = 0;


    private void Awake()
    {
        Debug.Log("QuickSystem Awake");
        for (int i = 0; i < capacity; ++i)
        {
            slots[i] = new QuickSlot();
        }

        Managers.Game.Player.Input.InputActions.Player.QuickSlot.started += OnQuickUseInput;
    }

    private void Start()
    {
        Debug.Log("QuickSystem Start");
    }

    public void Regist(int index, QuickSlot slot)
    {
        slot.itemSlot.SetRegist(true);
        slots[index].Set(slot.targetIndex, slot.itemSlot);

        OnUpdated?.Invoke(index, slot.itemSlot);
        OnRegisted?.Invoke(slot.targetIndex);

        if(index == IndexInUse)
        {
            IndexInUse = index;
            QuickUse();
        }
    }

    public void UnRegist(QuickSlot slot)
    {
        for(int i = 0; i <  slots.Length; ++i)
        {
            if (slot.targetIndex == slots[i].targetIndex)
            {
                slots[i].itemSlot.SetRegist(false);
                slots[i].Clear();
                OnUpdated?.Invoke(i, slots[i].itemSlot);
                OnUnRegisted?.Invoke(slot);
            }
        }

        // 손에 잡고 있는 오브젝트 였으면. 삭제
    }

    private void OnQuickUseInput(InputAction.CallbackContext context)
    {
        OnQuickUseInput((int)context.ReadValue<float>() - 1);
        QuickUse();
    }

    public void OnQuickUseInput(int index)
    {
        IndexInUse = index;
        QuickUse();
    }

    private void QuickUse()
    {
        if (slots[IndexInUse].itemSlot == null)
        {
            Managers.Game.Player.ToolSystem.ClearHand();
            return;
        }
        switch (slots[IndexInUse].itemSlot.itemData)
        {
            case ToolItemData _:
                Managers.Game.Player.ToolSystem.Equip(slots[IndexInUse].itemSlot);// targetIndex
                break;

            case ConsumeItemData _:
                break;

            default:
                break;
        }
    }
}
