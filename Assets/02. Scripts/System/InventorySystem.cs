using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySystem : MonoBehaviour
{
    public static int maxCapacity { get; } = 30;

    public ItemSlot[] slots { get; private set; }

    public Player Owner { get; private set; }

    public event Action<int, ItemSlot> OnUpdated;

    private UIInventory _inventoryUI;

    private void Awake()
    {
        Debug.Log("Inventory Awake");
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

    private void Start()
    {
        Debug.Log("Inventory Start");
        Managers.Game.Player.QuickSlot.OnRegisted += OnItemRegisted;
        Managers.Game.Player.QuickSlot.OnUnRegisted += OnItemUnregisted;
        Managers.Game.Player.ToolSystem.OnEquip += OnItemEquipped;
        Managers.Game.Player.ToolSystem.OnUnEquip += OnItemUnEquipped;
    }

    private void AddDefaultToolAsTest()
    {
        var itemData = Resources.Load<ScriptableObject>("SO/PickItemData") as ItemData;
        AddItem(itemData, 1);

        itemData = Resources.Load<ScriptableObject>("SO/AxeItemData") as ItemData;
        AddItem(itemData, 1);

        itemData = Resources.Load<ScriptableObject>("SO/SwordItemData") as ItemData;
        AddItem(itemData, 1);

        itemData = Resources.Load<ScriptableObject>("SO/EmptyHandItemData") as ItemData;
        AddItem(itemData, 1);
    }

    public void AddItem(ItemData itemData, int quantity)
    {
        int targetindex = 0;
        
        if(itemData.stackable == false)
        {
            if(FindEmptyIndex(out targetindex))
            {
                slots[targetindex].Set(itemData);
                OnUpdated?.Invoke(targetindex, slots[targetindex]);
                return;
            }
        }

        var itemSlot = FindItem(itemData, out targetindex);
        if(itemSlot != null)
        {
            itemSlot.AddQuantity(quantity);
            OnUpdated?.Invoke(targetindex, itemSlot);
            return;
        }

        if(FindEmptyIndex(out targetindex))
        {
            slots[targetindex].Set(itemData);
            OnUpdated?.Invoke(targetindex, slots[targetindex]);
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
    public void DestroyItemByIndex(QuickSlot quickSlot)
    {
        int index = quickSlot.targetIndex;
        if (slots[index].equipped || slots[index].registed) return;

        slots[index].Clear();
        OnUpdated?.Invoke(index, slots[index]);
    }

    public void OnItemEquipped(QuickSlot slot)
    {
        int index = slot.targetIndex;
        slots[index].SetEquip(slot.itemSlot.equipped);
        OnUpdated?.Invoke(index, slots[index]);
    }

    public void OnItemUnEquipped(QuickSlot slot)
    {
        int index = slot.targetIndex;
        slots[index].SetEquip(slot.itemSlot.equipped);
        OnUpdated?.Invoke(index, slots[index]);
    }

    // 밑에 2개 하나로 합쳐도 될듯?
    public void OnItemRegisted(QuickSlot slot)
    {
        int index = slot.targetIndex;
        slots[index].SetRegist(slot.itemSlot.registed);
        OnUpdated?.Invoke(index, slots[index]);
    }

    public void OnItemUnregisted(QuickSlot slot)
    {
        int index = slot.targetIndex;
        slots[index].SetRegist(slot.itemSlot.registed);
        OnUpdated?.Invoke(index, slots[index]);
    }
    // 여기까지

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
