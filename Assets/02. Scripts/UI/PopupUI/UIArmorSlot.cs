using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
// Lee gyuseong 24.02.06

public class UIArmorSlot : UIItemSlot
{
    enum Images
    {
        Icon,
    }

    public ItemParts part;

    public override void Initialize()
    {
        Bind<Image>(typeof(Images));
        Get<Image>((int)Images.Icon).gameObject.SetActive(false);
        Get<Image>((int)Images.Icon).raycastTarget = false;
    }

    private void Awake()
    {
        Initialize();

        // Container로
        Managers.Game.Player.ArmorSystem.OnEquipArmor += EquipArmor;
        Managers.Game.Player.ArmorSystem.OnUnEquipArmor += UnEquipArmor;
    }

    public void EquipArmor(QuickSlot quickSlot)
    {
        if (quickSlot == null) return;
        int parts = GetPart(quickSlot);

        if (part == (ItemParts)parts)
        {
            Set(quickSlot.itemSlot);
        }
    }

    public void UnEquipArmor(QuickSlot quickSlot)
    {
        int parts = GetPart(quickSlot);

        if (part == (ItemParts)parts)
        {
            Clear();
        }
    }
    // Container로

    public override void Set(ItemSlot itemSlot)
    {
        Get<Image>((int)Images.Icon).sprite = itemSlot.itemData.iconSprite;
        Get<Image>((int)Images.Icon).gameObject.SetActive(true);
    }

    public override void Clear()
    {
        Get<Image>((int)Images.Icon).gameObject.SetActive(false);
    }

    private int GetPart(QuickSlot slot)
    {
        var itemData = slot.itemSlot.itemData as EquipItemData;
        if (itemData == null) return -1;
        return (int)itemData.part;
    }
}
