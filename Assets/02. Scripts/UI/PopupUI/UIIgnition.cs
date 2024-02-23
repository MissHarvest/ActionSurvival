using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//Lee gyuseong 24.02.22

public class UIIgnition : UIPopup
{
    private enum GameObjects
    {
        Exit,
        UIFunctionsUseFireSlot,
        FirewoodItems
    }

    enum Helper
    {
        UIStoreFirewoodHelper
    }

    private GameObject _functionsUseFireSlotButton;
    private Transform _firewoodItems;
    private UIStoreFirewoodHelper _firewoodHelper;
    private Slider _firePowerGaugeSlider;
    private DayCycle _dayCycle;
    private Ignition _ignition;

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

        _functionsUseFireSlotButton.BindEvent((x) => { ShowCookingUIPopup(); });
        _firePowerGaugeSlider = GetComponentInChildren<Slider>();
    }

    private void OnEnable()
    {
        _ignition = GameManager.Instance.Player.Ignition;
        _dayCycle = GameManager.DayCycle;

        Initialize();

        _firewoodItems = Get<GameObject>((int)GameObjects.FirewoodItems).transform;
        _firewoodHelper = Get<UIStoreFirewoodHelper>((int)Helper.UIStoreFirewoodHelper);

        _ignition.OnUpdatedUI += OnSetIngredients;
        _ignition.OnUpdatedUI += OnUpdateFirePowerGaugeSlider;
        _dayCycle.OnTimeUpdated += OnUpdateFirePowerGaugeSlider;

        OnSetIngredients();
    }

    private void Start()
    {        
        gameObject.SetActive(false);
    }

    private void OnUpdateFirePowerGaugeSlider()
    {
        _firePowerGaugeSlider.value = _ignition._firePowerGauge;
    }

    public void OnSetIngredients()
    {
        ClearItems();

        foreach (var item in _ignition.firewoodItemSlots)
        {
            var prefab = Managers.Resource.GetCache<GameObject>("UIFirewoodSlot.prefab");
            GameObject itemUI = Instantiate(prefab, _firewoodItems);
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

        for (int i = 0; i < _ignition.firewoodItemSlots.Length; i++)
        {
            if (_ignition.firewoodItemSlots[i].itemData == itemSlot.itemData)
            {
                index = i;
            }
        }

        //여기서 Index를 넘겨주는 방법
        var pos = new Vector3(_firewoodItems.position.x - 100, _firewoodItems.position.y, _firewoodItems.position.z);
        _firewoodHelper.ShowOption(itemSlot, pos, index);
    }

    private void ShowCookingUIPopup()
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
