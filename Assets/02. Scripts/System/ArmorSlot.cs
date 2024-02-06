using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// Lee gyuseong 24.02.06

public class ArmorSlot : UIItemSlot
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
        Managers.Game.Player.ToolSystem.OnEquip += EquipArmor;
        Managers.Game.Player.ToolSystem.OnUnEquip += UnEquipArmor;
    }

    public void EquipArmor(QuickSlot quickSlot)
    { 
        Set(quickSlot.itemSlot);
    }

    public void UnEquipArmor(QuickSlot quickSlot)
    {
        Clear();
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
}
