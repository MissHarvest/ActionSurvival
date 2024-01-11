using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeItemSlot : UIBase
{
    enum Images
    {
        Icon,
    }

    protected Image Icon => Get<Image>((int)Images.Icon);

    public override void Initialize()
    {
        Bind<Image>(typeof(Images));

        Get<Image>((int)Images.Icon).raycastTarget = false;

        Clear();
    }

    private void Awake()
    {
        Initialize();
    }

    public virtual void Set(ItemSlot itemSlot)
    {
        if (itemSlot.itemData == null)
        {
            Clear();
            return;
        }

        Get<Image>((int)Images.Icon).sprite = itemSlot.itemData.iconSprite;
        Get<Image>((int)Images.Icon).gameObject.SetActive(true);
    }

    public virtual void Clear()
    {
        Get<Image>((int)Images.Icon).gameObject.SetActive(false);
    }
}
