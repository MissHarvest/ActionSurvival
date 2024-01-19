using System;
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
        var itemData = Managers.Resource.GetCache<ItemData>("PickAxeItemData.data");
        AddItem(itemData, 1);

        itemData = Managers.Resource.GetCache<ItemData>("AxeItemData.data");
        AddItem(itemData, 1);

        itemData = Managers.Resource.GetCache<ItemData>("SwordItemData.data");
        AddItem(itemData, 1);

        itemData = Managers.Resource.GetCache<ItemData>("EmptyHandItemData.data");
        AddItem(itemData, 1);

        itemData = Managers.Resource.GetCache<ItemData>("GreatswordItemData.data");
        AddItem(itemData, 1);

        itemData = Managers.Resource.GetCache<ItemData>("TwinDaggerItemData.data");
        AddItem(itemData, 1);

        // 고급 레시피 테스트용 재료
        itemData = Managers.Resource.GetCache<ItemData>("LowStoneItemData.data");
        AddItem(itemData, 1);
        itemData = Managers.Resource.GetCache<ItemData>("CraftingTableItemData.data");
        AddItem(itemData, 1);
        itemData = Managers.Resource.GetCache<ItemData>("BonFireItemData.data");
        AddItem(itemData, 1);
        itemData = Managers.Resource.GetCache<ItemData>("RabbitMeatItemData.data");
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
        //Debug.Log("인덱스 : " + index);
        slots[index].SetRegist(slot.itemSlot.registed);

        OnUpdated?.Invoke(index, slots[index]);
    }
    // 여기까지

    // 인벤토리의 소비 아이템 Use
    public void UseItemByIndex(int index, bool fromQuickSlot = false)
    {
        ItemSlot targetSlot;

        if (fromQuickSlot)
        {
            // 퀵슬롯의 소비 아이템이 인벤토리의 몇 번째 슬롯에 있는지 확인
            int inventoryIndex = Managers.Game.Player.QuickSlot.slots[index].targetIndex;
            index = inventoryIndex;
            targetSlot = slots[inventoryIndex];
        }
        else
        {
            targetSlot = slots[index];
        }

        var consume = targetSlot.itemData as ConsumeItemData;
        var conditionHandler = Owner.ConditionHandler;

        foreach (var playerCondition in consume.conditionModifier)
        {
            switch (playerCondition.Condition)
            {
                case Conditions.HP:
                    conditionHandler.HP.Add(playerCondition.value);
                    break;

                case Conditions.Hunger:
                    conditionHandler.Hunger.Add(playerCondition.value);
                    break;
            }
        }

        targetSlot.SubtractQuantity();
        OnUpdated?.Invoke(index, targetSlot);
    }

    // 퀵슬롯의 소비 아이템 Use
    public void UseSlotItemByIndex(int index)
    {
        UseItemByIndex(index, true);
    }

    // 인벤토리에서 Use버튼을 눌렀을 때 실행
    public void UseConsumeItemByIndex(int index)
    {
        // 인벤토리 인덱스
        int inventoryIndex = index;

        // 퀵슬롯 인덱스
        int quickSlotIndex = -1;
        for (int i = 0; i < Managers.Game.Player.QuickSlot.slots.Length; i++)
        {
            if (index == Managers.Game.Player.QuickSlot.slots[i].targetIndex)
            {
                quickSlotIndex = i;
                break;
            }
        }

        // 인벤토리에서 아이템 사용
        UseItemByIndex(inventoryIndex);

        // 아이템 수량 감소 및 QuickSlot 업데이트
        Managers.Game.Player.QuickSlot.slots[quickSlotIndex].itemSlot.SubtractQuantity();

        // 아이템 수량이 0이 되면 퀵슬롯에서 제거
        if (Managers.Game.Player.QuickSlot.slots[quickSlotIndex].itemSlot.quantity <= 0)
        {
            Managers.Game.Player.QuickSlot.UnRegist(Managers.Game.Player.QuickSlot.slots[quickSlotIndex]);
        }
        else
        {
            // 아이템 수량이 0이 아니면 QuickSlot 업데이트
            Managers.Game.Player.QuickSlot.UpdateQuickSlot(quickSlotIndex);
        }
    }


    // 특정 아이템의 개수 반환
    public int GetItemCount(ItemData itemData)
    {
        int count = 0;
        foreach (var slot in slots)
        {
            if (slot.itemData == itemData)
            {
                count += slot.quantity;
            }
        }
        return count;
    }

    public void RemoveItem(ItemData itemData, int quantity)
    {
        for (int i = 0; i < slots.Length; ++i)
        {
            if (slots[i].itemData == itemData)
            {
                // 해당 아이템의 수량을 감소시키고, 수량이 0 이하로 떨어지면 해당 슬롯을 비움
                slots[i].SubtractQuantity(quantity);
                if (slots[i].quantity <= 0)
                {
                    slots[i].Clear();
                    //OnUpdated?.Invoke(i, slots[i]);
                }
                OnUpdated?.Invoke(i, slots[i]);
                return;
            }
        }
        Debug.Log($"아이템 ({itemData.name})이 인벤토리에 없어요.");
    }
    public bool IsFull()
    {
        foreach (var slot in slots)
        {
            // 슬롯이 비어있거나, 아이템이 이미 존재하면서 수량이 최대 스택 수에 도달하지 않은 경우
            if (slot.itemData == null || (slot.itemData.stackable && slot.quantity < ItemData.maxStackCount))
            {
                return false;
            }
        }
        return true;
    }
}
