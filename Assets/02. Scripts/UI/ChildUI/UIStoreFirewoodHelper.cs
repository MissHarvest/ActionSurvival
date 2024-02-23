using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStoreFirewoodHelper : UIBase
{
    //UI를 그리는 기능만 남기고 MakeFire로 옮기기
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


    private UIMakeFire _makeFireUI;
    private MakeFire _makeFire;

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

        _minusButton.BindEvent((x) => { _makeFire.OnMinusQuantity(); });
        _plusButton.BindEvent((x) => { _makeFire.OnPlusQuantity(); });
        _takeoutButton.BindEvent((x) => { _makeFire.OnTakeoutQuantity(); });
        _storeButton.BindEvent((x) => { _makeFire.OnStoreQuantity(); });
        gameObject.BindEvent((x) => { gameObject.SetActive(false); });
    }

    private void Awake()
    {
        _makeFire = GameManager.Instance.Player.MakeFire;
        Initialize();
        gameObject.SetActive(false);
        _makeFireUI = GetComponentInParent<UIMakeFire>();
    }

    private void Start()
    {
        _makeFire.OnUpdatedCount += UpdateCountUI;
        _makeFire.OnUpdatedUI += OnTurnOffPopup;
    }

    public void ShowOption(ItemSlot selectedSlot, Vector3 position, int index)
    {
        int FirewoodHaveInventory = GameManager.Instance.Player.Inventory.GetItemCount(selectedSlot.itemData);
        _makeFire._maxCount = FirewoodHaveInventory;

        gameObject.SetActive(true);
        if (gameObject.activeSelf)
        {
            _makeFire._count = 0;
            UpdateCountUI(_makeFire._count);
        }

        _icon.sprite = selectedSlot.itemData.iconSprite;
        _quantityHaveText.text = "보유:" + FirewoodHaveInventory.ToString();

        _container.transform.position = position;
        _makeFire._index = index;
    }    

    public void UpdateCountUI(int count)
    {
        _quantityText.text = count.ToString();
    }

    private void OnTurnOffPopup()
    {
        gameObject.SetActive(false);
    }
}
