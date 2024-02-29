using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
// Lee gyuseong 24.02.06

public class UIArmorSlot : UIItemSlot
{
    public Sprite[] sprites;
    public Color emptyState;

    enum Images
    {
        Icon,
    }

    public ItemParts part;

    public override void Initialize()
    {
        Bind<Image>(typeof(Images));
        Clear();
        Get<Image>((int)Images.Icon).raycastTarget = false;
    }

    private void Awake()
    {
        Initialize();
    }

    public void SetPart(ItemParts part)
    {
        this.part = part;
        Clear();
    }

    public override void Set(ItemSlot itemSlot)
    {
        if (itemSlot.itemData == null)
        {
            Clear();
            return;
        }
        Get<Image>((int)Images.Icon).color = Color.white;
        Get<Image>((int)Images.Icon).sprite = itemSlot.itemData.iconSprite;
        Get<Image>((int)Images.Icon).gameObject.SetActive(true);
    }

    public override void Clear()
    {
        Get<Image>((int)Images.Icon).sprite = sprites[(int)part];
        Get<Image>((int)Images.Icon).color = emptyState;
    }
}
