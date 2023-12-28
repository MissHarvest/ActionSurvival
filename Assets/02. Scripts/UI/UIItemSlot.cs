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

    private Image _itemIcon;
    private TextMeshProUGUI _quantityText;
    public int index { get; private set; }

    public override void Initialize()
    {
        
    }

    private void Awake()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        _itemIcon = transform.Find("Icon").GetComponent<Image>();
        if(_itemIcon == null)
        {
            Debug.LogWarning($"ItemSlotUI[{index}] dont have Icon Object or Image Component");
        }
        _itemIcon.gameObject.SetActive(false);
        Get<Image>((int)Images.Icon).gameObject.BindEvent((x) => 
        {
            var helper = Managers.UI.ShowPopupUI<UIItemUsageHelper>();
            helper.ShowOption(index, transform);
        });
        
        _quantityText = transform.Find("Quantity").GetComponent<TextMeshProUGUI>();
        if(_quantityText == null)
        {
            Debug.LogWarning($"ItemSlotUI[{index}] dont have Quantity Object or TextMeshPro Component");
        }
        _quantityText.text = string.Empty;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void Set(ItemSlot itemSlot)
    {
        if(itemSlot.itemData == null)
        {
            _itemIcon.gameObject.SetActive(false);
            _quantityText.gameObject.SetActive(false);
            return;
        }

        _itemIcon.sprite = itemSlot.itemData.iconSprite;
        _itemIcon.gameObject.SetActive(true);

        _quantityText.text = itemSlot.quantity.ToString();
        _quantityText.gameObject.SetActive(itemSlot.itemData.stackable);
    }
}
