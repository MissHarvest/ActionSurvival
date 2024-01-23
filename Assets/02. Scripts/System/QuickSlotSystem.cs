using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickSlotSystem : MonoBehaviour
{
    public static int capacity = 4;
    public QuickSlot[] slots = new QuickSlot[capacity];

    public event Action<int, ItemSlot> OnUpdated;
    public event Action<QuickSlot> OnRegisted;
    public event Action<QuickSlot> OnUnRegisted;

    public int IndexInUse { get; private set; } = -1;

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
        Managers.Game.Player.Inventory.OnUpdated += OnInventoryUpdated;
    }

    public void Regist(int index, QuickSlot slot)
    {
        slot.itemSlot.SetRegist(true);
        slots[index].Set(slot.targetIndex, slot.itemSlot);
        Debug.Log($"Quick | R[{slots[index].itemSlot.registed}] E[{slots[index].itemSlot.equipped}]");
        OnUpdated?.Invoke(index, slot.itemSlot);
        OnRegisted?.Invoke(slot);

        if (index == IndexInUse)
        {
            //IndexInUse = index;
            QuickUse();
        }
    }

    public void UnRegist(QuickSlot slot)
    {
        for (int i = 0; i < slots.Length; ++i)
        {
            if (slot.targetIndex == slots[i].targetIndex)
            {
                slot.itemSlot.SetRegist(false);
                OnUnRegisted?.Invoke(slot);
                slots[i].Clear();
                OnUpdated?.Invoke(i, slots[i].itemSlot);
                return;
            }
        }
    }

    private void OnQuickUseInput(InputAction.CallbackContext context)
    {
        OnQuickUseInput((int)context.ReadValue<float>() - 1);
    }

    public void OnQuickUseInput(int index)
    {
        if (index != IndexInUse)
        {
            IndexInUse = index;
            QuickUse();
        }
    }

    private void QuickUse()
    {
        if (IndexInUse == -1) return;
        if (slots[IndexInUse].itemSlot.itemData == null)
        {
            Debug.Log($"Quick Use {IndexInUse}");
            IndexInUse = -1;
            Managers.Game.Player.ToolSystem.ClearHand();
            return;
        }

        switch (slots[IndexInUse].itemSlot.itemData)
        {
            case ToolItemData _:
                Managers.Game.Player.ToolSystem.Equip(slots[IndexInUse]);
                break;

            case ConsumeItemData _:
                Managers.Game.Player.Inventory.UseConsumeItemByIndex(slots[IndexInUse].targetIndex);
                break;

            default:
                break;
        }
    }

    // 퀵슬롯 인덱스 번호로 업데이트
    public void UpdateQuickSlot(int index)
    {
        IndexInUse = index;
        int inventoryIndex = slots[IndexInUse].targetIndex;
        slots[IndexInUse].Set(inventoryIndex, Managers.Game.Player.Inventory.slots[inventoryIndex]);
        OnUpdated?.Invoke(IndexInUse, slots[IndexInUse].itemSlot);
    }

    public void OnInventoryUpdated(int inventoryIndex, ItemSlot itemSlot)
    {
        for (int i = 0; i < capacity; ++i)
        {
            if (slots[i].targetIndex == inventoryIndex)
            {
                slots[i].Set(slots[i].targetIndex, itemSlot);
                OnUpdated?.Invoke(i, slots[i].itemSlot);
                if (itemSlot.itemData == null)
                {
                    IndexInUse = -1;
                    Managers.Game.Player.ToolSystem.ClearHand();
                }
                return;
            }
        }
    }
}
