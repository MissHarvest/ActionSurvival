using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BossMonster;
// Lee gyuseong 24.02.06

public class UIArmorSlot : UIItemSlot
{
    //여기선 UI적인 처리만 하기, 데이터 처리는 다른 클래스에서 . . .
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
        Managers.Game.Player.ToolSystem.OnEquip += EquipArmor;
        Managers.Game.Player.ToolSystem.OnUnEquip += UnEquipArmor;
        //Managers.Game.Player.Inventory.OnUpdated += UpdateArmorSlots;
    }

    public void EquipArmor(QuickSlot quickSlot)
    {
        int parts = GetPart(quickSlot);

        if (parts == 0)
        {
            switch (part)
            {
                case ItemParts.Head:
                    Set(quickSlot.itemSlot);
                    break;
            }
        }
        else if (parts == 1)
        {
            switch (part)
            {
                case ItemParts.Body:
                    Set(quickSlot.itemSlot);
                    break;
            }
        }
    }

    public void UnEquipArmor(QuickSlot quickSlot)
    {
        int parts = GetPart(quickSlot);

        if (parts == 0)
        {
            switch (part)
            {
                case ItemParts.Head:
                    Clear();
                    break;
            }
        }
        else if (parts == 1)
        {
            switch (part)
            {
                case ItemParts.Body:
                    Clear();
                    break;
            }
        }
    }

    //public void UpdateArmorSlots(int index, ItemSlot itemSlot)
    //{
    //    var itemData = itemSlot.itemData as EquipItemData;

    //    int parts = (int)itemData.part;

    //    //itemSlot = Managers.Game.Player.Inventory.slots[index];


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
