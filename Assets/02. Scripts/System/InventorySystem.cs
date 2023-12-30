using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySystem : MonoBehaviour
{
    public static int maxCapacity { get; } = 30;

    public ItemSlot[] slots;

    public Player Owner { get; private set; }

    public event Action<int, ItemSlot> OnUpdated;

    private UIInventory _inventoryUI;

    private void Awake()
    {
        slots = new ItemSlot[maxCapacity];
        for(int i = 0; i < slots.Length; ++i)
        {
            slots[i] = new ItemSlot();
        }

        Owner = Managers.Game.Player;

        var input = Owner.Input;
        input.InputActions.Player.Inventory.started += OnInventoryShowAndHide;

        // TEST //
        AddDefaultToolAsTest();
    }

    private void AddDefaultToolAsTest()
    {
        var itemData = Resources.Load<ScriptableObject>("SO/PickItemData") as ItemData;
        AddItem(new ItemSlot(itemData, 1));

        itemData = Resources.Load<ScriptableObject>("SO/AxeItemData") as ItemData;
        AddItem(new ItemSlot(itemData, 1));

        itemData = Resources.Load<ScriptableObject>("SO/SwordItemData") as ItemData;
        AddItem(new ItemSlot(itemData, 1));
    }

    public void AddItem(ItemSlot slot)
    {
        int targetindex = 0;

        if(slot.itemData.stackable == false)
        {
            if(FindEmptyIndex(out targetindex))
            {
                slots[targetindex] = slot;
                OnUpdated?.Invoke(targetindex, slot);
                return;
            }
        }

        var itemSlot = FindItem(slot.itemData, out targetindex);
        if(itemSlot != null)
        {
            itemSlot.AddQuantity(slot.quantity);
            OnUpdated?.Invoke(targetindex, slot);
            return;
        }

        if(FindEmptyIndex(out targetindex))
        {
            slots[targetindex] = slot;
            OnUpdated?.Invoke(targetindex, slot);
        }
    }

    private bool FindEmptyIndex(out int index)
    {
        // FindItem 사용해서 구현할 수 있을지도
        for(int i = 0; i < slots.Length; ++i)
        {
            if (slots[i].itemData == null)
            {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }

    public ItemSlot FindItem(ItemData itemData, out int index)
    {
        for(int i = 0; i < slots.Length; ++i)
        {
            if (slots[i].itemData == itemData && slots[i].IsFull == false)
            {
                index = i;
                return slots[i];
            }    
        }
        index = -1;
        return null;
    }

    private void OnInventoryShowAndHide(InputAction.CallbackContext context)
    {
        if (_inventoryUI == null)
        {
            _inventoryUI = Managers.UI.ShowPopupUI<UIInventory>();
            return;
        }

        if (_inventoryUI.gameObject.activeSelf)
        {
            Managers.UI.ClosePopupUI(_inventoryUI);
        }
        else
        {
            Managers.UI.ShowPopupUI<UIInventory>();
        }
    }


    // Item Control // >> 다른 클래스로 빼낼 수 있을려나
    public void DestroyItemByIndex(int index)
    {
        slots[index].Clear();
        OnUpdated?.Invoke(index, slots[index]);
    }

    public void EquipItemByIndex(int index)
    {
        slots[index].bUse = true; // bUse 접근 제한자 고민하기
        Owner.ToolSystem.Equip(slots[index]);
        OnUpdated?.Invoke(index, slots[index]); // 착용 또는 등록시 E / R 표시하는 기능 추가할 예정
    }

    public void UnEquipItemByIndex(int index)
    {
        slots[index].bUse = false; // bUse 접근 제한자 고민하기
        Owner.ToolSystem.UnEquip();
        OnUpdated?.Invoke(index, slots[index]); // 착용 또는 등록시 E / R 표시하는 기능 추가할 예정
    }

    public void RegisteItemByIndex(int index)
    {

    }

    public void UseItemByIndex(int index)
    {
        var consume = slots[index].itemData as ConsumeItemData;
        var conditionHandler = Owner.ConditionHandler;

        foreach(var playerCondition in consume.conditionModifier)
        {
            switch(playerCondition.Condition) 
            {
                case Conditions.HP:
                    conditionHandler.HP.Add(playerCondition.value);
                    break;

                case Conditions.Hunger:
                    conditionHandler.Hunger.Add(playerCondition.value);
                    break;
            }
        }

        slots[index].SubtractQuantity();
        OnUpdated?.Invoke(index, slots[index]);
    }
}
