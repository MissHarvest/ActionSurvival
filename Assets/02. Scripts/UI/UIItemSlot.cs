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

    public int index { get; private set; }
    public RectTransform RectTransform { get; private set; }

    public override void Initialize()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        RectTransform = GetComponent<RectTransform>();
    }

    private void Awake()
    {
        Initialize();

        var iconObject = Get<Image>((int)Images.Icon).gameObject;
        iconObject.SetActive(false);
        iconObject.BindEvent((x) => 
        {
            var helper = Managers.UI.ShowPopupUI<UIItemUsageHelper>();
            helper.ShowOption(index, new Vector3(transform.position.x + RectTransform.sizeDelta.x, transform.position.y));
        });

        var quantityText = Get<TextMeshProUGUI>((int)Texts.Quantity);
        quantityText.text = string.Empty;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void Set(ItemSlot itemSlot)
    {
        if(itemSlot.itemData == null)
        {
            Get<Image>((int)Images.Icon).gameObject.SetActive(false);
            Get<TextMeshProUGUI>((int)Texts.Quantity).gameObject.SetActive(false);
            return;
        }

        Get<Image>((int)Images.Icon).sprite = itemSlot.itemData.iconSprite;
        Get<Image>((int)Images.Icon).gameObject.SetActive(true);

        Get<TextMeshProUGUI>((int)Texts.Quantity).text = itemSlot.quantity.ToString();
        Get<TextMeshProUGUI>((int)Texts.Quantity).gameObject.SetActive(itemSlot.itemData.stackable);
    }
}
