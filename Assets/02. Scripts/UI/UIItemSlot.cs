using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour
{
    private Image _itemIcon;
    private TextMeshProUGUI _quantityText;
    public int index { get; private set; }

    private void Awake()
    {
        _itemIcon = transform.Find("Icon").GetComponent<Image>();
        if(_itemIcon == null)
        {
            Debug.LogWarning($"ItemSlotUI[{index}] dont have Icon Object or Image Component");
        }
        _itemIcon.gameObject.SetActive(false);

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
        _itemIcon.sprite = itemSlot.itemData.iconSprite;
        _itemIcon.gameObject.SetActive(_itemIcon.sprite != null);

        _quantityText.text = itemSlot.quantity.ToString();
        _quantityText.gameObject.SetActive(itemSlot.quantity != 0);
    }
}
