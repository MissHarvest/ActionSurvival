using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// µî·Ï¿ë
public class UIQuickSlot : UIBase
{
    enum Images
    {
        Icon,
    }

    enum Texts
    {
        Quantity,
    }

    private UIQuickSlotSelector _container;
    private int _index;

    public override void Initialize()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        Clear();

        gameObject.BindEvent((x) =>
        {
            OnClicked();
        });
    }

    private void Awake()
    {
        Initialize();
    }

    public void Set(int index, UIQuickSlotSelector quickSlotSelector)
    {
        _index = index;
        _container = quickSlotSelector;        
    }

    public void Set(ItemSlot itemSlot)
    {
        if (itemSlot.itemData == null)
        {
            Clear();
            return;
        }

        var icon = Get<Image>((int)Images.Icon);
        icon.sprite = itemSlot.itemData.iconSprite;
        icon.gameObject.SetActive(true);

        if(itemSlot.itemData.stackable)
        {
            var quantity = Get<TextMeshProUGUI>((int)Texts.Quantity);
            quantity.text = itemSlot.quantity.ToString();
            quantity.gameObject.SetActive(true);
        }
    }

    private void OnClicked()
    {
        Managers.Game.Player.QuickSlot.Regist(_index, _container.sourceIndex);
        Managers.UI.ClosePopupUI(_container);
    }

    private void Clear()
    {
        Get<Image>((int)Images.Icon).gameObject.SetActive(false);
        Get<TextMeshProUGUI>((int)Texts.Quantity).gameObject.SetActive(false);
    }
}
