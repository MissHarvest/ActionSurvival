using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
// Lee gyuseong 24.02.14

public class UIStoreFirewoodHelper : UIBase
{
    public enum GameObjects
    {
        Container,
        MinusButton,
        PlusButton,
        TakeoutButton,
        StoreButton
    }

    enum Texts
    {
        QuantityText,
        QuantityHaveText
    }

    enum Images
    {
        Icon,
    }

    private GameObject _container;
    private GameObject _minusButton;
    private GameObject _plusButton;
    private GameObject _takeoutButton;
    private GameObject _storeButton;

    private TextMeshProUGUI _quantityText;
    private TextMeshProUGUI _quantityHaveText;

    private Image _icon;
    private int _count = 0;
    private int _maxCount;
    private int _index;

    private UIMakeFire _makeFireUI;

    public override void Initialize()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));

        _container = Get<GameObject>((int)GameObjects.Container);
        _minusButton = Get<GameObject>((int)GameObjects.MinusButton);
        _plusButton = Get<GameObject>((int)GameObjects.PlusButton);
        _takeoutButton = Get<GameObject>((int)GameObjects.TakeoutButton);
        _storeButton = Get<GameObject>((int)GameObjects.StoreButton);

        _quantityText = Get<TextMeshProUGUI>((int)Texts.QuantityText);
        _quantityHaveText = Get<TextMeshProUGUI>((int)Texts.QuantityHaveText);

        _icon = Get<Image>((int)Images.Icon);

        _minusButton.BindEvent((x) => { OnMinusQuantity(); });
        _plusButton.BindEvent((x) => { OnPlusQuantity(); });
        _takeoutButton.BindEvent((x) => { OnTakeoutQuantity(); });
        _storeButton.BindEvent((x) => { OnStoreQuantity(); });
        gameObject.BindEvent((x) => { gameObject.SetActive(false); });
    }

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
        _makeFireUI = GetComponentInParent<UIMakeFire>();
    }

    public void ShowOption(ItemSlot selectedSlot, Vector3 position, int index)
    {
        int FirewoodHaveInventory = Managers.Game.Player.Inventory.GetItemCount(selectedSlot.itemData);
        _maxCount = FirewoodHaveInventory;

        gameObject.SetActive(true);
        if (gameObject.activeSelf)
        {
            _count = 0;
            UpdateCraftUI();
        }

        _icon.sprite = selectedSlot.itemData.iconSprite;
        _quantityHaveText.text = "보유:" + FirewoodHaveInventory.ToString();

        _container.transform.position = position;
        _index = index;
    }

    private void OnTakeoutQuantity()
    {
        var itemSlots = _makeFireUI.itemSlots[_index];

        if (itemSlots.quantity > 0)
        {
            Managers.Game.Player.Inventory.AddItem(itemSlots.itemData, itemSlots.quantity);
            _makeFireUI.itemSlots[_index].FirewoodItemSubtractQuantity(itemSlots.quantity);

            _makeFireUI.SetIngredients();

            gameObject.SetActive(false);
        }
    }

    private void OnStoreQuantity()
    {
        if (_count > 0)
        {
            _makeFireUI.itemSlots[_index].AddQuantity(_count);
            Managers.Game.Player.Inventory.RemoveItem(_makeFireUI.itemSlots[_index].itemData, _count);

            _makeFireUI.SetIngredients();

            gameObject.SetActive(false);
        }        
    }

    private void OnMinusQuantity()
    {
        if (_count > 0)
        {
            _count--;
            UpdateCraftUI();
        }
    }

    private void OnPlusQuantity()
    {
        if (_count < _maxCount)
        {
            _count++;
            UpdateCraftUI();
        }
    }

    private void UpdateCraftUI()
    {
        _quantityText.text = _count.ToString();
    }
}
