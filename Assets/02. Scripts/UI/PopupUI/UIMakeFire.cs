using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// Lee gyuseong 24.02.13

public class UIMakeFire : UIPopup
{
    //내부에 존재하는 각 버튼들을 알맞게 이벤트 바인딩을 해준다.

    private enum GameObjects
    {
        Exit,
        UIFunctionsUseFireSlot,
        FirewoodItems
    }

    private GameObject _functionsUseFireSlotButton;
    private Transform _firewoodItems;
    [SerializeField] private GameObject _itemSlotPrefab;
    [SerializeField] private ItemData[] _itemSlots = new ItemData[2];

    public override void Initialize()
    {
        base.Initialize();

        Bind<GameObject>(typeof(GameObjects));
        Get<GameObject>((int)GameObjects.Exit).BindEvent((x) =>
        {
            Managers.UI.ClosePopupUI(this);
        });

        _functionsUseFireSlotButton = Get<GameObject>((int)GameObjects.UIFunctionsUseFireSlot);

        _functionsUseFireSlotButton.BindEvent((x) => { OnCookingUIPopup(); });
    }

    private void Awake()
    {
        Initialize();
        GetFirewoodItems();

        _firewoodItems = Get<GameObject>((int)GameObjects.FirewoodItems).transform;

        gameObject.SetActive(false);
    }

    private void Start()
    {
        SetIngredients();
    }

    private void SetIngredients()
    {
        foreach (var item in _itemSlots)
        {
            GameObject itemUI = Instantiate(_itemSlotPrefab, _firewoodItems);
            Image itemIcon = itemUI.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI itemQuantity = itemUI.GetComponentInChildren<TextMeshProUGUI>();

            // 아이템 아이콘 설정
            itemIcon.sprite = item.iconSprite;

            itemQuantity.text = 0.ToString();
        }
    }

    private void GetFirewoodItems()
    {
        var itemData = Managers.Resource.GetCache<ItemData>("BranchItemData.data");
        _itemSlots[0] = itemData;
        itemData = Managers.Resource.GetCache<ItemData>("LogItemData.data");
        _itemSlots[1] = itemData;
    }

    private void OnCookingUIPopup()
    {
        Managers.Game.Player.Cooking.OnCookingShowAndHide();
    }
}
