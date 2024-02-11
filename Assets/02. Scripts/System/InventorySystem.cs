using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySystem : MonoBehaviour
{
    [field: SerializeField] public int maxCapacity { get; private set; } = 30;

    [field: SerializeField] public ItemSlot[] slots { get; private set; }

    public Player Owner { get; private set; }

    public event Action<int, ItemSlot> OnUpdated;
    public event Action<ItemSlot> OnItemAdd;

    protected virtual void Awake()
    {
        Debug.Log($"Inventory Awake [{gameObject.name}] [{this.name}]");
        slots = new ItemSlot[maxCapacity];

        for (int i = 0; i < slots.Length; ++i)
        {
            slots[i] = new ItemSlot(this);
        }

        Owner = Managers.Game.Player;

        Managers.Game.OnSaveCallback += Save;
    }

    public void AddDefaultToolAsTest()
    {

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
                OnItemAdd?.Invoke(itemslot);
                OnUpdated?.Invoke(targetIndex, slots[targetIndex]);
                return;
            }
        }

        var itemSlot = FindItem(itemslot.itemData, out int targetindex);
        if (itemSlot != null)
        {
            itemSlot.AddQuantity(itemslot.quantity);
            OnItemAdd?.Invoke(itemslot);
            OnUpdated?.Invoke(targetindex, itemSlot);
            return;
        }

        if (FindEmptyIndex(out targetindex))
        {
            slots[targetindex].Set(itemslot);
            OnItemAdd?.Invoke(itemslot);
            OnUpdated?.Invoke(targetindex, slots[targetindex]);
        }

        // stack 이 가능하면 > 있는지 확인 
    }

    public void AddItem(ItemData itemData, int quantity)
    {
        int targetindex = 0;

        if (itemData.stackable == false)
        {
            if (FindEmptyIndex(out targetindex))
            {
                slots[targetindex].Set(itemData);
                OnItemAdd?.Invoke(new ItemSlot(itemData, quantity));
                OnUpdated?.Invoke(targetindex, slots[targetindex]);
                return;
            }
        }

        var itemSlot = FindItem(itemData, out targetindex);
        if (itemSlot != null)
        {
            itemSlot.AddQuantity(quantity);
            OnItemAdd?.Invoke(new ItemSlot(itemData, quantity));
            OnUpdated?.Invoke(targetindex, itemSlot);
            return;
        }

        if (FindEmptyIndex(out targetindex))
        {
            slots[targetindex].Set(itemData, quantity);
            OnItemAdd?.Invoke(new ItemSlot(itemData, quantity));
            OnUpdated?.Invoke(targetindex, slots[targetindex]);
        }
    }

    private bool FindEmptyIndex(out int index)
    {
        // FindItem 사용해서 구현할 수 있을지도
        for (int i = 0; i < slots.Length; ++i)
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
        for (int i = 0; i < slots.Length; ++i)
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

    public bool IsFull(ItemData completedItem, int quantity)
    {
        int totalQuantity = 0;

        for (int i = 0; i < slots.Length; ++i)
        {
            if (!completedItem.stackable)
            {
                if (slots[i].itemData == null)
                {
                    return false;
                }
            }
            else
            {
                // 스택 가능한 아이템의 경우, 빈 슬롯 또는 같은 아이템이 있는 슬롯을 찾아 확인
                if (slots[i].itemData == null)
                {
                    return false;
                }
                else if (slots[i].itemData == completedItem && slots[i].quantity < ItemData.maxStackCount)
                {
                    totalQuantity += slots[i].quantity;
                }
            }
        }

        // 아이템이 들어갈 자리가 있는지와 수량을 함께 확인
        if (totalQuantity + quantity <= ItemData.maxStackCount)
        {
            return false;
        }

        Debug.Log("아이템이 들어갈 자리가 없어요");
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

    public virtual void Load()
    {
        SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, $"{gameObject.name}Inventory");
        foreach (var item in slots)
        {
            if (item.itemName != string.Empty)
                item.LoadData();
        }
    }

    protected virtual void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile($"{gameObject.name}Inventory", json, SaveGame.SaveType.Runtime);
    }
}
