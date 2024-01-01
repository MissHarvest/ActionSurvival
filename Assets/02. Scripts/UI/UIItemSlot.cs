using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlot : UIBase
{
    enum Images
    {
        Icon,
    }

    enum Texts
    {
        Quantity,
    }

    protected Image Icon => Get<Image>((int)Images.Icon);
    protected TextMeshProUGUI Quantity => Get<TextMeshProUGUI>((int)Texts.Quantity);

    public override void Initialize()
    {
        Bind<Image>(typeof(Images));        
        Bind<TextMeshProUGUI>(typeof(Texts));

        Get<Image>((int)Images.Icon).raycastTarget = false;
        Get<TextMeshProUGUI>((int)Texts.Quantity).raycastTarget = false;

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

        if (0 != itemSlot.quantity)
        {
            Get<TextMeshProUGUI>((int)Texts.Quantity).text = itemSlot.quantity.ToString();
            Get<TextMeshProUGUI>((int)Texts.Quantity).gameObject.SetActive(itemSlot.itemData.stackable);
        }
    }

    public virtual void Clear()
    {
        Get<Image>((int)Images.Icon).gameObject.SetActive(false);
        Get<TextMeshProUGUI>((int)Texts.Quantity).gameObject.SetActive(false);
    }
}
