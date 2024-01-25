using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySystem : MonoBehaviour
{
    public int maxCapacity { get; private set; } = 30;

    public ItemSlot[] slots { get; private set; }
    public Player Owner { get; private set; }

    public event Action<int, ItemSlot> OnUpdated;
    public event Action<ItemData> OnItemAdd;

    private void Awake()
    {
        Debug.Log($"Inventory Awake [{gameObject.name}] [{this.name}]");
        slots = new ItemSlot[maxCapacity];

        for(int i = 0; i < slots.Length; ++i)
        {
            slots[i] = new ItemSlot(this);
        }

        Owner = Managers.Game.Player;
    }

    public void AddDefaultToolAsTest()
    {
        var itemData = Managers.Resource.GetCache<ItemData>("PickAxeItemData.data");
        AddItem(itemData, 1);

        itemData = Managers.Resource.GetCache<ItemData>("AxeItemData.data");
        AddItem(itemData, 1);

        itemData = Managers.Resource.GetCache<ItemData>("WoodSwordItemData.data");
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
        itemData = Managers.Resource.GetCache<ItemData>("FenceItemData.data");
        AddItem(itemData, 1);
    }

    public void SetCapacity(int capacity)
    {
        maxCapacity = capacity;
    }

    public void AddItem(ItemSlot itemslot)
    {
        int targetIndex = 0;

        // stack 가능한가?
        if (itemslot.itemData.stackable == false)
        {
            if (FindEmptyIndex(out targetIndex))
            {
                slots[targetIndex].Set(itemslot);
                OnItemAdd?.Invoke(itemslot.itemData);
                OnUpdated?.Invoke(targetIndex, slots[targetIndex]);
                return;
            }
        }

        var itemSlot = FindItem(itemslot.itemData, out int targetindex);
        if (itemSlot != null)
        {
            itemSlot.AddQuantity(itemslot.quantity);
            OnItemAdd?.Invoke(itemslot.itemData);
            OnUpdated?.Invoke(targetindex, itemSlot);
            return;
        }

        if (FindEmptyIndex(out targetindex))
        {
            slots[targetindex].Set(itemslot);
            OnItemAdd?.Invoke(itemslot.itemData);
            OnUpdated?.Invoke(targetindex, slots[targetindex]);
        }

        // stack 이 가능하면 > 있는지 확인 
    }

    public void AddItem(ItemData itemData, int quantity)
    {
        int targetindex = 0;
        
        if(itemData.stackable == false)
        {
            if(FindEmptyIndex(out targetindex))
            {
                slots[targetindex].Set(itemData);
                OnItemAdd?.Invoke(itemData);
                OnUpdated?.Invoke(targetindex, slots[targetindex]);
                return;
            }
        }

        var itemSlot = FindItem(itemData, out targetindex);
        if(itemSlot != null)
        {
            itemSlot.AddQuantity(quantity);
            OnItemAdd?.Invoke(itemData);
            OnUpdated?.Invoke(targetindex, itemSlot);
            return;
        }

        if(FindEmptyIndex(out targetindex))
        {
            slots[targetindex].Set(itemData, quantity);
            OnItemAdd?.Invoke(itemData);
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
    
    public void TransitionItem(InventorySystem targetInventory, int index)
    {
        if (slots[index].equipped || slots[index].registed)
        {
            var ui = Managers.UI.ShowPopupUI<UIWarning>();
            ui.SetWarning("Unregist or UnEquip item");
            return;// 경고문
        }
        targetInventory.AddItem(slots[index]);
        slots[index].Clear();
        OnUpdated?.Invoke(index, slots[index]);
    }


    protected void BroadCastUpdatedSlot(int index, ItemSlot itemslot)
    {
        OnUpdated?.Invoke(index, itemslot);
    }
}
