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
        Owner = GetComponentInParent<Player>();
        Load();
    }
    private void Start()
    {
        Owner.QuickSlot.OnRegisted += OnItemRegisted;
        Owner.QuickSlot.OnUnRegisted += OnItemRegisted;
        Owner.ToolSystem.OnEquip += OnItemEquipped;
        Owner.ToolSystem.OnUnEquip += OnItemEquipped;
        Owner.Building.OnBuildCompleted += UseArchitectureItem;
        Owner.ArmorSystem.OnEquipArmor += OnItemEquipped;
        Owner.ArmorSystem.OnUnEquipArmor += OnItemEquipped;
        AddDefaultToolAsTest();
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
    public bool TrySubtractDurability(int index, float amount)
    {
        var slot = Get(index);
        var able = slot.itemData != null && slot.itemData.MaxDurability != 0.0f;
        if (able)
        {
            var itemdata = slot.itemData;
            slot.SubtractDurability(amount);
            BroadCastUpdatedSlot(index, slot);
            if(slot.quantity == 0)
                UpdateDic(itemdata);
        }
        return able;
    }

    /* 내구도 + 아이템 데이터 : 아마 필요해질수도.. */

    public void DestroyItem(int index)
    {
        var itemSlot = Get(index);
        if (itemSlot.equipped || itemSlot.registed)
        {
            var ui = Managers.UI.ShowPopupUI<UIWarning>();
            ui.SetWarning("등록했거나, 착용 중인 아이템은 버릴 수 없습니다.");
            return;
        }
        var quantity = itemSlot.quantity;
        TryConsumeQuantity(index, quantity);
    }

    private void UseArchitectureItem(int index)
    {
        StartCoroutine(DelayedUseArchitectureItem(index));
    }

    private IEnumerator DelayedUseArchitectureItem(int index)
    {
        float delayInSeconds = 0.3f;
        yield return new WaitForSeconds(delayInSeconds);

        TryConsumeQuantity(index, 1);
    }

    private void OnItemEquipped(QuickSlot slot)
    {
        int index = slot.targetIndex;
        var newSlot = Get(index);
        if (newSlot.itemData == null) return;
        newSlot.SetEquip(slot.itemSlot.equipped);
        BroadCastUpdatedSlot(index, newSlot);
    }

    private void OnItemRegisted(QuickSlot slot)
    {
        int index = slot.targetIndex;
        var newSlot = Get(index);
        if (newSlot.itemData == null) return;
        newSlot.SetRegist(slot.itemSlot.registed);
        BroadCastUpdatedSlot(index, newSlot);
    }
}
