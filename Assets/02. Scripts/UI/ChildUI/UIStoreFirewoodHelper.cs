using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//Lee gyuseong 24.02.22

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

    private Ignition _ignition;

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

        _minusButton.BindEvent((x) => { _ignition.OnMinusQuantity(); });
        _plusButton.BindEvent((x) => { _ignition.OnPlusQuantity(); });
        _takeoutButton.BindEvent((x) => { _ignition.OnTakeoutQuantity(); });
        _storeButton.BindEvent((x) => { _ignition.OnStoreQuantity(); });
        gameObject.BindEvent((x) => { gameObject.SetActive(false); });
    }

    private void OnEnable()
    {
        _ignition = GameManager.Instance.Player.Ignition;

        Initialize();

        _ignition.OnUpdatedCount += UpdateCountUI;
        _ignition.OnUpdatedUI += OnTurnOffPopup;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowOption(ItemSlot selectedSlot, Vector3 position, int index)
    {
        int FirewoodHaveInventory = GameManager.Instance.Player.Inventory.GetItemCount(selectedSlot.itemData);
        _ignition._maxCount = FirewoodHaveInventory;

        gameObject.SetActive(true);
        if (gameObject.activeSelf)
        {
            _ignition._count = 0;
            UpdateCountUI(_ignition._count);
        }

        _icon.sprite = selectedSlot.itemData.iconSprite;
        _quantityHaveText.text = "보유:" + FirewoodHaveInventory.ToString();

        _container.transform.position = position;
        _ignition._index = index;
    }    

    public void UpdateCountUI(int count)
    {
        _quantityText.text = count.ToString();
        Debug.Log(count);
    }

    private void OnTurnOffPopup()
    {
        gameObject.SetActive(false);
    }
}
