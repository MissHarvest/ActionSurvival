using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMakeFire : UIPopup
{
    //내부에 존재하는 각 버튼들을 알맞게 이벤트 바인딩을 해준다.
    //Data 가지고 있지 않기

    private enum GameObjects
    {
        Exit,
        UIFunctionsUseFireSlot,
        FirewoodItems,
        UIFirewoodSlot
    }

    enum Helper
    {
        UIStoreFirewoodHelper
    }

    private GameObject _functionsUseFireSlotButton;
    private Transform _firewoodItems;
    private UIStoreFirewoodHelper _firewoodHelper;
    [SerializeField] private GameObject _itemSlotPrefab;

    public ItemSlot[] itemSlots = new ItemSlot[2];
    private List<GameObject> _itemUIList = new List<GameObject>();

    public override void Initialize()
    {
        base.Initialize();

        Bind<GameObject>(typeof(GameObjects));
        Bind<UIStoreFirewoodHelper>(typeof(Helper));

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
        _firewoodHelper = Get<UIStoreFirewoodHelper>((int)Helper.UIStoreFirewoodHelper);
    }

    public void SetIngredients()
    {
        ClearItems();

        foreach (var item in itemSlots)
        {
            GameObject itemUI = Instantiate(_itemSlotPrefab, _firewoodItems);
            Image itemIcon = itemUI.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI itemQuantity = itemUI.GetComponentInChildren<TextMeshProUGUI>();

            itemIcon.sprite = item.itemData.iconSprite;
            itemQuantity.text = item.quantity.ToString();

            itemUI.BindEvent((x) => { ShowStoreFirewoodPopupUI(item); });

            _itemUIList.Add(itemUI);
        }
    }

    private void ShowStoreFirewoodPopupUI(ItemSlot itemSlot)
    {
        int index = 0;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].itemData == itemSlot.itemData)
            {
                index = i;
            }
        }

        //여기서 Index를 넘겨주는 방법
        var pos = new Vector3(_firewoodItems.position.x - 100, _firewoodItems.position.y, _firewoodItems.position.z);
        _firewoodHelper.ShowOption(itemSlot, pos, index);
    }

    private void GetFirewoodItems()
    {
        var itemData = Managers.Resource.GetCache<ItemData>("BranchItemData.data");
        itemSlots[0].Set(itemData, 0);
        itemData = Managers.Resource.GetCache<ItemData>("LogItemData.data");
        itemSlots[1].Set(itemData, 0);
    }

    private void OnCookingUIPopup()
    {
        var ui = Managers.UI.ShowPopupUI<UICooking>();
        //ui.SetAdvancedRecipeUIActive(_cookingLevel);
    }

    private void ClearItems()
    {
        foreach (GameObject itemUI in _itemUIList)
        {
            Destroy(itemUI);
        }
    }
}
