using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BossMonster;
// Lee gyuseong 24.02.06

public class UIArmorSlot : UIItemSlot
{
    ////여기선 UI적인 처리만 하기, 데이터 처리는 다른 클래스에서 . . .
    ////UIArmorSlot을 관리하는 객체가 이벤트 구독
    ////ItemData == null 이면 null인 상태의 아이콘을 그려볼까나
    ////UIInventory가 UIItemSlotContainer를 쓰는 것 처럼?
    ////UIInventorySlot

    //public event Action<QuickSlot> OnUnEquipped;

    //enum Images
    //{
    //    Icon,
    //}

    //public ItemParts part;

    //public override void Initialize()
    //{
    //    Bind<Image>(typeof(Images));
    //    Get<Image>((int)Images.Icon).gameObject.SetActive(false);
    //    Get<Image>((int)Images.Icon).raycastTarget = false;
    //}

    //private void Awake()
    //{
    //    Initialize();
    //    Managers.Game.Player.ToolSystem.OnEquip += EquipArmor;
    //    Managers.Game.Player.ToolSystem.OnUnEquip += UnEquipArmor;
    //}

    //public void EquipArmor(QuickSlot quickSlot)
    //{
    //    int parts = GetPart(quickSlot);

    //    if (parts == 0)
    //    {
    //        switch (part)
    //        {
    //            case ItemParts.Head:
    //                Set(quickSlot.itemSlot);
    //                break;
    //        }
    //    }
    //    else if (parts == 1)
    //    {
    //        switch (part)
    //        {
    //            case ItemParts.Body:
    //                Set(quickSlot.itemSlot);
    //                break;
    //        }
    //    }
    //}

    //public void UnEquipArmor(QuickSlot quickSlot)
    //{
    //    int parts = GetPart(quickSlot);

    //    if (parts == 0)
    //    {
    //        switch (part)
    //        {
    //            case ItemParts.Head:
    //                Clear();
    //                break;
    //        }
    //    }
    //    else if (parts == 1)
    //    {
    //        switch (part)
    //        {
    //            case ItemParts.Body:
    //                Clear();
    //                break;
    //        }
    //    }
    //}    

    //public override void Set(ItemSlot itemSlot)
    //{
    //    Get<Image>((int)Images.Icon).sprite = itemSlot.itemData.iconSprite;
    //    Get<Image>((int)Images.Icon).gameObject.SetActive(true);
    //}

    //public override void Clear()
    //{
    //    Get<Image>((int)Images.Icon).gameObject.SetActive(false);
    //}

    //private int GetPart(QuickSlot slot)
    //{
    //    var itemData = slot.itemSlot.itemData as EquipItemData;
    //    if (itemData == null) return -1;
    //    return (int)itemData.part;
    //}    
}
