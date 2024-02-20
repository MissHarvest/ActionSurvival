using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Lee gyuseong 24.02.15

public class UIArmorSlotContainer : MonoBehaviour
{
    private List<UIArmorSlot> _armorSlots = new List<UIArmorSlot>();

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
            for (int i = 0; i < _armorSlots.Count; i++)
            {
                _armorSlots[i].EquipArmor(armor);
            }
        }
    }

    private void SetParts(UIArmorSlot armorSlotUI, int index)
    {
        armorSlotUI.part = (ItemParts)index;
    }
}
