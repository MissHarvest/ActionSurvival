using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventorySystem : InventorySystem
{
    protected override void Awake()
    {
        base.Awake();
        Load();
    }
    private void Start()
    {
        Owner.QuickSlot.OnRegisted += OnItemRegisted;
        Owner.QuickSlot.OnUnRegisted += OnItemUnregisted;
        Owner.ToolSystem.OnEquip += OnItemEquipped;
        Owner.ToolSystem.OnUnEquip += OnItemUnEquipped;
    }
    
    public void DestroyItemByIndex(QuickSlot quickSlot)
    {
        int index = quickSlot.targetIndex;
        if (slots[index].equipped || slots[index].registed)
        {
            var ui = Managers.UI.ShowPopupUI<UIWarning>();
            ui.SetWarning("등록했거나, 착용 중인 아이템은 버릴 수 없습니다.");
            return;
        }

        slots[index].Clear();
        BroadCastUpdatedSlot(index, slots[index]);
    }

    public void UseArchitectureItem(int index)
    {
        slots[index].SubtractQuantity();
        BroadCastUpdatedSlot(index, slots[index]);
    }

    private void OnItemEquipped(QuickSlot slot)
    {
        int index = slot.targetIndex;
        slots[index].SetEquip(slot.itemSlot.equipped);
        BroadCastUpdatedSlot(index, slots[index]);
    }

    private void OnItemUnEquipped(QuickSlot slot)
    {
        int index = slot.targetIndex;
        if (slots[index].itemData == null) return;
        slots[index].SetEquip(slot.itemSlot.equipped);
        BroadCastUpdatedSlot(index, slots[index]);
    }

    // 밑에 2개 하나로 합쳐도 될듯?
    private void OnItemRegisted(QuickSlot slot)
    {
        int index = slot.targetIndex;
        slots[index].SetRegist(slot.itemSlot.registed);
        BroadCastUpdatedSlot(index, slots[index]);
    }

    private void OnItemUnregisted(QuickSlot slot)
    {
        int index = slot.targetIndex;
        slots[index].SetRegist(slot.itemSlot.registed);

        BroadCastUpdatedSlot(index, slots[index]);
    }

    public void UseConsumeItemByIndex(int index)
    {
        // 인벤토리 인덱스
        int inventoryIndex = index;

        // 허기 채워줌
        ItemSlot targetSlot = slots[inventoryIndex];
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

        slots[inventoryIndex].SubtractQuantity();
        BroadCastUpdatedSlot(inventoryIndex, targetSlot);
    }

    public void UseToolItemByIndex(int index, float amount)
    {
        float currentDurability = slots[index].currentDurability;
        slots[index].SetDurability(currentDurability - amount);
        ItemSlot targetSlot = slots[index];
        BroadCastUpdatedSlot(index, targetSlot);
    }

    public override void Load()
    {
        if (SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "PlayerInventory") == false)
        {
            AddDefaultToolAsTest();
            //var itemData = Managers.Resource.GetCache<ItemData>("StoneItemData.data");
            //AddItem(itemData, 20);
        }
    }

    protected override void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile("PlayerInventory", json, SaveGame.SaveType.Runtime);
    }
}
