using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
//Lee gyuseong 24.02.22

public class UIIgnition : UIPopup
{
    private enum GameObjects
    {
        Exit,
        UIFunctionsUseFireSlot,
        FirewoodItems,
        Content
    }

    enum Helper
    {
        UIStoreFirewoodHelper
    }

    private GameObject _functionsUseFireSlotButton;
    private Transform _firewoodItems;
    private Transform _content;
    private UIStoreFirewoodHelper _firewoodHelper;
    private Slider _firePowerGaugeSlider;
    private DayCycle _dayCycle;
    private Ignition _ignition;

    private List<GameObject> _itemUIList = new List<GameObject>();
    public List<GameObject> _cookingSlot = new List<GameObject>();

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
        _content = Get<GameObject>((int)GameObjects.Content).transform;
        _firewoodHelper = Get<UIStoreFirewoodHelper>((int)Helper.UIStoreFirewoodHelper);

        _ignition.OnUpdatedUI += OnSetIngredients;
        _ignition.OnUpdatedUI += OnUpdateFirePowerGaugeSlider;
        _dayCycle.OnTimeUpdated += OnUpdateFirePowerGaugeSlider;
        //_dayCycle.OnTimeUpdated += OnUpdatedTimeTakenToCookSlider;
        _ignition.OnUpdateQuantity += OnUpdateQuantity;
        _ignition.OnUpdateQuantity += OnUpdateList;

        OnSetIngredients();
        CreatCookingSlot();
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

    public void CreatCookingSlot()
    {
        if (_ignition._cookingSlotsUI.Count == 0)
        {            
            for (int i = 0; i < _ignition.recipeRequiredItemSlots.Length; i++)
            {
                var prefab = Managers.Resource.GetCache<GameObject>("UICookingSlot.prefab");
                GameObject itemUI = Instantiate(prefab, _content);
                var cookingSlot = itemUI.GetComponent<UICookingSlot>();

                cookingSlot.Set(_ignition.recipeRequiredItemSlots[i]);
                cookingSlot._cookedFoodItemQuantity.text = "0";
                cookingSlot.index = i;

                itemUI.BindEvent((x) => DeliverFoodItemsToInventory(_ignition._cookingSlotsUI[i].index));

                _ignition._cookingSlotsUI.Add(cookingSlot);
            }
        }
    }

    public void Set(int index)
    {
        foreach (var recipe in _ignition.recipes)
        {
            if (recipe.completedItemData == _ignition.recipeRequiredItemSlots[index].itemData)
            {
                var cookingSlot = _ignition._cookingSlotsUI[index];
                cookingSlot.Set(_ignition.recipeRequiredItemSlots[index]);
                cookingSlot.SetMaxTimeTakenToCookSlider(recipe.maxTimeRequiredToCook);
                _ignition._cookingSlotsUI[index]._maxTimeRequiredToCook = recipe.maxTimeRequiredToCook;
            }
        }
    }

    private void OnUpdateQuantity(int index)
    {
        if (_ignition._cookingSlotsUI[index] == null) return;

        var cookingSlot = _ignition._cookingSlotsUI[index].GetComponent<UICookingSlot>();
        cookingSlot.Set(_ignition.recipeRequiredItemSlots[index]);
        cookingSlot.CookedFoodItemQuantitySet(_ignition.cookedFoodItems[index]);
    }

    public void OnUpdatedTimeTakenToCookSlider()
    {
        if (_ignition._cookingSlotsUI.Count == 0) return;

        for (int i = 0; i < _ignition._cookingSlotsUI.Count; i++)
        {
            if (_ignition._cookingSlotsUI[i] != null)
            {
                var cookingSlot = _ignition._cookingSlotsUI[i].GetComponent<UICookingSlot>();
                cookingSlot.UpdatedTimeTakenToCookSlider(_ignition._currentTimeRequiredToCook);
            }            
        }        
    }

    public void OnUpdateList(int index)
    {

    }

    private void DeliverFoodItemsToInventory(int index)//
    {
        //_ignition.cookedFoodItems 에서 Inventory로 넘겨주기
        GameManager.Instance.Player.Inventory.TryAddItem(_ignition.cookedFoodItems[index].itemData, _ignition.cookedFoodItems[index].quantity);
        _ignition.cookedFoodItems[index].SubtractQuantity(_ignition.cookedFoodItems[index].quantity);
        var cookingSlot = _cookingSlot[index].GetComponent<UICookingSlot>();
        cookingSlot.CookedFoodItemQuantitySet(_ignition.cookedFoodItems[index]);

        if (_ignition.recipeRequiredItemSlots[index].itemData == null)
        {
            Destroy(_cookingSlot[index]);
            _cookingSlot[index] = null;

            for (int i = 0; i < _cookingSlot.Count; i++)
            {
                if (_ignition._cookingSlotsUI[i] == null)
                {
                    _ignition._cookingSlotsUI.Remove(_ignition._cookingSlotsUI[i]);
                }
                if (_cookingSlot[i] == null)
                {
                    _cookingSlot.Remove(_cookingSlot[i]);
                }
            }
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
