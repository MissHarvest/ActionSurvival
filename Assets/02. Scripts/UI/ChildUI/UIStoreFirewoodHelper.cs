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
    private GameObject _storeButton;

    private TextMeshProUGUI _quantityText;
    private TextMeshProUGUI _quantityHaveText;

    private Image _icon;
    private int _count = 0;
    private int _maxCount;

    private UIMakeFire _makeFireUI;

    public override void Initialize()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));

        _container = Get<GameObject>((int)GameObjects.Container);
        _minusButton = Get<GameObject>((int)GameObjects.MinusButton);
        _plusButton = Get<GameObject>((int)GameObjects.PlusButton);
        _storeButton = Get<GameObject>((int)GameObjects.StoreButton);

        _quantityText = Get<TextMeshProUGUI>((int)Texts.QuantityText);
        _quantityHaveText = Get<TextMeshProUGUI>((int)Texts.QuantityHaveText);

        _icon = Get<Image>((int)Images.Icon);

        _minusButton.BindEvent((x) => { OnMinusQuantity(); });
        _plusButton.BindEvent((x) => { OnPlusQuantity(); });
        _storeButton.BindEvent((x) => { OnStoreQuantity(); });
        gameObject.BindEvent((x) => { gameObject.SetActive(false); });
    }

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
        _makeFireUI = GetComponentInParent<UIMakeFire>();
    }

    public void ShowOption(ItemSlot selectedSlot, Vector3 position)
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
    }

    private void OnStoreQuantity()
    {
        //GetComponentParant로 UIMakeFire class를 받아와서 그 안의 _itemSlot의 데이터를 이용한다면
        //_itemSlot의 quantity는 _count 만큼 증가, Inventory의 quantity는 감소
        //빼기 버튼
        //요리가 등록되면 장작을 소모하여 점화버튼을 누를 수 있게 된다.
        //요리에 소요되는 시간이 추가된다.
        //요리가 끝나면 이미 활성화된 화력게이지는 되돌릴 수 없고 점화가 끝나면 장작을 더 소모하지 않는다.

        _makeFireUI.itemSlots[1].AddQuantity(_count);
        Managers.Game.Player.Inventory.RemoveItem(_makeFireUI.itemSlots[1].itemData , _count);

        _makeFireUI.SetIngredients();

        gameObject.SetActive(false);
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
