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
    public Sprite[] sprites;

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
    }

    public override void Set(ItemSlot itemSlot)
    {
        if (itemSlot.itemData == null) return;
        Get<Image>((int)Images.Icon).sprite = itemSlot.itemData.iconSprite;
        Get<Image>((int)Images.Icon).gameObject.SetActive(true);
    }

    public override void Clear()
    {
        Get<Image>((int)Images.Icon).sprite = sprites[(int)part];
    }
}
