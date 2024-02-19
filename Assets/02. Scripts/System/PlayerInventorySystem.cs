using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventorySystem : InventorySystem
{
    // 인벤토리 라기 보다는 아이템 사용 도우미가 맞는거 같음 //
    public Player Owner { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Owner = Managers.Game.Player;
        Debug.Log($"PlayerInventory Awake [{gameObject.name}] [{this.name}]");
        Load();
    }
    private void Start()
    {
        Owner.QuickSlot.OnRegisted += OnItemRegisted;
        Owner.QuickSlot.OnUnRegisted += OnItemUnregisted;
        Owner.ToolSystem.OnEquip += OnItemEquipped;
        Owner.ToolSystem.OnUnEquip += OnItemUnEquipped;
        Owner.Building.OnBuildCompleted += UseArchitectureItem;
    }

    public int GetIndexOfItem(ItemData itemData)
    {
        if (_itemDic.TryGetValue(itemData, out List<int> list))
        {
            return list[0];
        }
        return -1;
    }

    /// <summary>
    /// 인벤토리의 index 에 위치한 아이템의 내구도를 감소시킬 때,
    /// </summary>
    /// <param name="index"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool TryConsumeDurability(int index, float amount)
    {
        var able = _slots[index].itemData != null && _slots[index].itemData.MaxDurability != 0.0f;
        if (able)
        {
            _slots[index].SubtractDurability(amount);
            BroadCastUpdatedSlot(index, _slots[index]);
        }
        return able;
    }

    /* 내구도 + 아이템 데이터 : 아마 필요해질수도.. */

    public void DestroyItemByIndex(QuickSlot quickSlot)
    {
        int index = quickSlot.targetIndex;
        if (_slots[index].equipped || _slots[index].registed)
        {
            var ui = Managers.UI.ShowPopupUI<UIWarning>();
            ui.SetWarning("등록했거나, 착용 중인 아이템은 버릴 수 없습니다.");
            return;
        }
        var quantity = _slots[index].quantity;
        TryConsumeQuantity(index, quantity);
    }

    public void UseArchitectureItem(int index)
    {
        TryConsumeQuantity(index, 1);
    }

    private void OnItemEquipped(QuickSlot slot)
    {
        int index = slot.targetIndex;
        _slots[index].SetEquip(slot.itemSlot.equipped);
        BroadCastUpdatedSlot(index, _slots[index]);
    }

    private void OnItemUnEquipped(QuickSlot slot)
    {
        int index = slot.targetIndex;
        if (_slots[index].itemData == null) return;
        _slots[index].SetEquip(slot.itemSlot.equipped);
        BroadCastUpdatedSlot(index, _slots[index]);
    }

    // 밑에 2개 하나로 합쳐도 될듯?
    private void OnItemRegisted(QuickSlot slot)
    {
        int index = slot.targetIndex;
        _slots[index].SetRegist(slot.itemSlot.registed);
        BroadCastUpdatedSlot(index, _slots[index]);
    }

    private void OnItemUnregisted(QuickSlot slot)
    {
        int index = slot.targetIndex;
        _slots[index].SetRegist(slot.itemSlot.registed);

        BroadCastUpdatedSlot(index, _slots[index]);
    }

    public void UseConsumeItemByIndex(int index)
    {
        if (GetItemCount(index) <= 0) return;
        // 인벤토리 인덱스
        int inventoryIndex = index;

        // 허기 채워줌
        ItemSlot targetSlot = _slots[inventoryIndex];
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

        _slots[inventoryIndex].SubtractQuantity();
        BroadCastUpdatedSlot(inventoryIndex, targetSlot);
    }

    public void UseToolItemByIndex(int index, float amount)
    {
        float currentDurability = _slots[index].currentDurability;
        _slots[index].SetDurability(currentDurability - amount);
        ItemSlot targetSlot = _slots[index];
        BroadCastUpdatedSlot(index, targetSlot);
    }

    public override void Load()
    {
        if (SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "PlayerInventory"))
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
        else
        {
#if UNITY_EDITOR
            //AddDefaultToolAsTest();
#endif
        }
    }

    protected override void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile("PlayerInventory", json, SaveGame.SaveType.Runtime);
    }
}
