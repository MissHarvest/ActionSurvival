using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
using static BossMonster;
// Lee gyuseong 24.02.06 save 기능 추가 필요

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

        Managers.Game.Player.ArmorSystem.EquipArmor += EquipArmor;
        Managers.Game.Player.ArmorSystem.UnEquipArmor += UnEquipArmor;
    }

    public void EquipArmor(QuickSlot quickSlot)
    {
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
