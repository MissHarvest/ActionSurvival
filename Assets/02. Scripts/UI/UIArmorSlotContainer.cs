using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Lee gyuseong 24.02.15

public class UIArmorSlotContainer : MonoBehaviour
{
    private List<UIArmorSlot> _armorSlots = new List<UIArmorSlot>();

    private void Awake()
    {
        GameManager.Instance.Player.ArmorSystem.OnEquipArmor += EquipArmor;
        GameManager.Instance.Player.ArmorSystem.OnUnEquipArmor += UnEquipArmor;
    }

    public void CreatArmorSlots<T>(GameObject slotPrefab, int count) where T : UIArmorSlot
    {
        for (int i = 0; i < count; i++)
        {
            var armorSlotPrefab = Instantiate(slotPrefab, this.transform);
            var armorSlotUI = armorSlotPrefab.GetComponent<T>();
            SetParts(armorSlotUI, i);
            _armorSlots.Add(armorSlotUI);
        }
    }

    public void Init<T>(QuickSlot[] quickSlot) where T : UIArmorSlot
    {
        foreach (var armor in quickSlot)
        {
            EquipArmor(armor);
        }
    }

    private void SetParts(UIArmorSlot armorSlotUI, int index)
    {
        armorSlotUI.SetPart((ItemParts)index);
    }

    public void EquipArmor(QuickSlot quickSlot)
    {
        if (quickSlot.itemSlot.itemData == null) return;

        int parts = GetPart(quickSlot);
        _armorSlots[parts].Set(quickSlot.itemSlot);
    }

    public void UnEquipArmor(QuickSlot quickSlot)
    {
        int parts = GetPart(quickSlot);
        _armorSlots[parts].Clear();
    }

    private int GetPart(QuickSlot slot)
    {
        var itemData = slot.itemSlot.itemData as EquipItemData;
        if (itemData == null) return -1;
        return (int)itemData.part;
    }
}
