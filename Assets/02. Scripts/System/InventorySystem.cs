using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySystem : MonoBehaviour
{
    [field: SerializeField] public int maxCapacity { get; private set; } = 30;

    protected Dictionary<ItemData, List<int>> _itemDic = new();

    [field: SerializeField] protected ItemSlot[] _slots;

    [HideInInspector][SerializeField] private int _emptySlotCount;

    public event Action<int, ItemSlot> OnUpdated;
    public event Action<ItemData, int> OnItemAdded;

#if UNITY_EDITOR
    [Header("Cheat")]
    // 배열로 하자
    public ItemData test_item;
    public int test_quantity;
#endif


    protected virtual void Awake()
    {
        _slots = new ItemSlot[maxCapacity];

        for (int i = 0; i < _slots.Length; ++i)
        {
            _slots[i] = new ItemSlot(this);
        }

        Managers.Game.OnSaveCallback += Save;
    }

    public void AddDefaultToolAsTest()
    {
        //var itemData = Managers.Resource.GetCache<ItemData>("StoneItemData.data");
        //AddItem(itemData, 1);
        //itemData = Managers.Resource.GetCache<ItemData>("StoneItemData.data");
        //AddItem(itemData, 10);


        //itemData = Managers.Resource.GetCache<ItemData>("LogItemData.data");
        //AddItem(itemData, 10);
    }

    public void SetCapacity(int capacity)
    {
        maxCapacity = capacity;
    }

    public bool TryAddItem(ItemData itemData, int quantity)
    {
        return TryAddItem(itemData, quantity, 0.0f);
    }

    public bool TryAddItem(ItemSlot itemSlot)
    {
        var itemData = itemSlot.itemData;
        var quantity = itemSlot.quantity;
        var durability = itemSlot.currentDurability;
        return TryAddItem(itemData, quantity, durability);
    }

    private bool TryAddItem(ItemData itemData, int quantity, float durability)
    {
        if (CheckEnoughSlot(itemData, quantity))
        {
            AddItem(itemData, quantity, durability);
            return true;
        }
        else
        {
            Managers.UI.ShowPopupUI<UIWarning>().SetWarning(
                "인벤토리에 공간이 부족합니다.");
        }
        return false;
    }

    private void AddItem(ItemData data, int quantity, float durability)
    {
        OnItemAdded?.Invoke(data, quantity);

        if (_itemDic.TryGetValue(data, out List<int> indexList))
        {
            for (int i = 0; i < indexList.Count; ++i)
            {
                var index = indexList[i];
                if (_slots[index].IsFull) continue;
                quantity = _slots[index].AddQuantity(quantity);
                OnUpdated?.Invoke(index, _slots[index]);
                if (quantity == 0) return;
            }
        }

        // 빈슬롯 찾아서 넣기
        for (int i = 0; i < _slots.Length; ++i)
        {
            if (_slots[i].itemData != null) continue;
            _slots[i].Set(data, durability);
            quantity = _slots[i].AddQuantity(quantity);

            if (_itemDic.TryGetValue(data, out indexList))
            {
                indexList.Add(i);
            }
            else
            {
                _itemDic.Add(data, new List<int>() { i });
            }
            OnUpdated?.Invoke(i, _slots[i]);
            if (quantity == 0) return;
        }
    }

    private bool CheckEnoughSlot(ItemData data, int quantity)
    {
        var remain = quantity;
        
        if(_itemDic.TryGetValue(data, out List<int> indexList))
        {
            for(int i = 0; i < indexList.Count;++i)
            {
                var index = indexList[i];
                if (_slots[index].IsFull) continue;
                var sub = data.MaxStackCount - _slots[index].quantity;
                remain -= sub;
                if (remain == 0) return true;
            }
        }
                
        var fCount = (float)remain / data.MaxStackCount;
        int count = Mathf.CeilToInt(fCount);
        return GetEmptySlotCount() >= count;
    }

    private int GetEmptySlotCount()
    {
        var count = 0;
        for(int i = 0; i < _slots.Length; ++i)
        {
            if (_slots[i].itemData == null)
            {
                ++count;
            }
        }
        return count;
    }

    public int GetItemCount(ItemData itemData)
    {
        int count = 0;
        if (_itemDic.TryGetValue(itemData, out List<int> indexList))
        {
            for (int i = 0; i < indexList.Count; ++i)
            {
                var index = indexList[i];
                count += GetItemCount(index);
            }
        }
        return count;
    }

    public int GetItemCount(int index)
    {
        if(_slots[index].itemData != null)
        {
            return _slots[index].quantity;
        }
        return 0;
    }

    /// <summary>
    /// 인벤토리에 있는 itemData 의 수량을 quantity 만큼 감소 시킬 때,
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public bool TryConsumeQuantity(ItemData itemData, int quantity)
    {
        var count = GetItemCount(itemData);
        if (count < quantity)
        {
            Managers.UI.ShowPopupUI<UIWarning>().SetWarning(
                "아이템 수량이 부족합니다.");
            return false;
        }

        if (_itemDic.TryGetValue(itemData, out List<int> indexList))
        {
            for (int i = 0; i < indexList.Count; ++i)
            {
                var index = indexList[i];
                quantity = _slots[index].SubtractQuantity(quantity);
                BroadCastUpdatedSlot(index, _slots[index]);
                if (quantity == 0) break;
            }
            UpdateDic(itemData);
        }
        return true;
    }

    /// <summary>
    /// 인벤토리의 index 에 위치한 아이템의 수량을 감소시킬 때,
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool TryConsumeQuantity(int index, int quantity = 1) 
    {
        var able = GetItemCount(index) >= quantity;
        if (able)
        {
            var itemData = _slots[index].itemData;
            _slots[index].SubtractQuantity(quantity);
            BroadCastUpdatedSlot(index, _slots[index]);
            if (_slots[index].quantity == 0)
                UpdateDic(itemData);
        }
        return able;
    }

    public ItemSlot Get(int index)
    {
        return _slots[index];
    }

    private void UpdateDic(ItemData itemData)
    {
        if(_itemDic.TryGetValue(itemData, out List<int> indexList))
        {
            _itemDic[itemData] = indexList.Where(x => _slots[x].itemData != null).ToList();
        }
#if UNITY_EDITOR
        PrintDic();
#endif
    }


    /// <summary>
    /// [사용되지 않는 함수]
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="quantity"></param>
    public void AddItem_Before(ItemData itemData, int quantity)
    {
        int targetindex = 0;

        if (itemData.stackable == false)
        {
            if (FindEmptyIndex(out targetindex))
            {
                _slots[targetindex].Set(itemData);
                //OnItemAdded?.Invoke(new ItemSlot(itemData, quantity));
                OnUpdated?.Invoke(targetindex, _slots[targetindex]);
                return;
            }
        }

        var itemSlot = FindItem(itemData, out targetindex);
        if (itemSlot != null)
        {
            itemSlot.AddQuantity(quantity);
            //OnItemAdded?.Invoke(new ItemSlot(itemData, quantity));
            OnUpdated?.Invoke(targetindex, itemSlot);
            return;
        }

        if (FindEmptyIndex(out targetindex))
        {
            _slots[targetindex].Set(itemData, quantity);
            //OnItemAdded?.Invoke(new ItemSlot(itemData, quantity));
            OnUpdated?.Invoke(targetindex, _slots[targetindex]);
        }
    }

    /// <summary>
    /// [사용되지 않는 함수]
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool FindEmptyIndex(out int index)
    {
        //for (int i = 0; i < slots.Length; ++i)
        //{
        //    if (slots[i].itemData == null)
        //    {
        //        index = i;
        //        return true;
        //    }
        //}
        index = -1;
        return false;
    }

    /// <summary>
    /// [사용되지 않는 함수]
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public ItemSlot FindItem(ItemData itemData, out int index)
    {
        for (int i = 0; i < _slots.Length; ++i)
        {
            if (_slots[i].itemData == itemData && _slots[i].IsFull == false)
            {
                index = i;
                return _slots[i];
            }
        }
        index = -1;
        return null;
    }
        
    /// <summary>
    /// [사용하지 않는 함수]
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="quantity"></param>
    public void RemoveItem(ItemData itemData, int quantity)
    {
        /*
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
        */
    }

    /// <summary>
    /// 사용되지 않는 함수
    /// </summary>
    /// <param name="completedItem"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public bool IsFull(ItemData completedItem, int quantity)
    {
        /*
        int totalCanStackQuantity = 0;
        int emptySlots = 0;

        for (int i = 0; i < slots.Length; ++i)
        {
            if (!completedItem.stackable)
            {
                // 스택 불가능한 아이템의 경우, quantity만큼 빈 슬롯이 있어야 함
                if (slots[i].itemData == null && quantity > 0)
                {
                    quantity--;
                }
            }
            else
            {
                int canStackQuantity = 0;
                // 스택 가능한 아이템의 경우, 빈 슬롯 또는 같은 아이템이 있는 슬롯을 찾아 확인
                if (slots[i].itemData == null)
                {
                    emptySlots++;
                }
                else if (slots[i].itemData == completedItem)
                {
                    canStackQuantity = completedItem.MaxStackCount - slots[i].quantity;
                    totalCanStackQuantity += canStackQuantity;
                }
            }
        }

        if (!completedItem.stackable && quantity > 0)
        {
            Debug.Log($"아이템 ({completedItem.name})이 들어갈 자리가 없어요");
            return true;
        }

        if (completedItem.stackable)
        {
            if (emptySlots * completedItem.MaxStackCount + totalCanStackQuantity < quantity)
            {
                Debug.Log($"아이템 ({completedItem.name})이 들어갈 자리가 없어요");
                return true;
            }
        }
        */
        return false;
    }

    public void TransitionItem(InventorySystem targetInventory, int index) // 특정 갯수만 하게 되면?
    {
        var quantity = _slots[index].quantity;
        TransitionItem(targetInventory, index, quantity);
    }

    public void TransitionItem(InventorySystem targetInventory, int index, int quantity)
    {
        if (GetItemCount(index) < quantity) return;        

        if (targetInventory.TryAddItem(_slots[index]))
        {
            TryConsumeQuantity(index, quantity);
        }
        OnUpdated?.Invoke(index, _slots[index]);
    }

    protected void BroadCastUpdatedSlot(int index, ItemSlot itemslot)
    {
        OnUpdated?.Invoke(index, itemslot);
    }

    public virtual void Load()
    {
        if(SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, $"{gameObject.name}Inventory"))
        {
            for (int i = 0; i < _slots.Length; ++i)
            {
                if (_slots[i].itemName != string.Empty)
                {
                    _slots[i].LoadData();
                    if (_itemDic.TryGetValue(_slots[i].itemData, out List<int> indexList))
                    {
                        indexList.Add(i);
                    }
                    else
                    {
                        _itemDic.TryAdd(_slots[i].itemData, new List<int>() { i });
                    }
                }
            }
        }
    }

    // add parameter file name 
    protected virtual void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile($"{gameObject.name}Inventory", json, SaveGame.SaveType.Runtime);
    }

#if UNITY_EDITOR
    private void PrintDic()
    {
        foreach (var data in _itemDic)
        {
            string line = $"[{data.ToString()}] ";
            for (int i = 0; i < data.Value.Count; ++i)
            {
                line += $"{data.Value[i]}/";
            }
            Debug.Log(line);
        }
    }

    public void TestAddItem()
    {
        if (test_item == null) return;
        TryAddItem(test_item, test_quantity);
    }

    public void TestRemoveItem()
    {
        if (test_item == null) return;
        TryConsumeQuantity(test_item, test_quantity);
    }

    public void TestClearInventory()
    {
        for(int i = 0; i < _slots.Length; ++i)
        {
            _slots[i].Clear();
            OnUpdated?.Invoke(i, _slots[i]);
        }
    }

    public void TestSave()
    {
        Save();
    }
#endif
}
