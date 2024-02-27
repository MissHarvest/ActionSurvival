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
    public Ignition ignition;

    [SerializeField] private int _cookingLevel;

    private List<GameObject> _itemUIList = new List<GameObject>();
    public Dictionary<string, Ignition> ignitionDic = new Dictionary<string, Ignition>();

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

        _dayCycle = GameManager.DayCycle;
        _cookingLevel = ignition.cookingLevel;

        foreach (var dic in ignitionDic)
        {
            Debug.Log(dic.Key);
        }
    }

    private void OnEnable()
    {
        //활성화 됐을 떄 동작하게
        //이건 데이터를 가져와서 그리기만 한다
        Initialize();

        _firewoodItems = Get<GameObject>((int)GameObjects.FirewoodItems).transform;
        _content = Get<GameObject>((int)GameObjects.Content).transform;
        _firewoodHelper = Get<UIStoreFirewoodHelper>((int)Helper.UIStoreFirewoodHelper);
        _firewoodHelper.ignition = ignition;

        ignition.OnUpdatedUI += OnSetIngredients;
        ignition.OnUpdatedUI += OnUpdateFirePowerGaugeSlider;
        ignition.OnUpdateQuantity += OnUpdateQuantity;
        //구독해지

        OnSetIngredients();
        CreatCookingSlot();
        ShowCookingSlots(_cookingLevel);
        OnUpdatedTimeTakenToCookSlider();
    }

    private void OnDisable()
    {
        for (int i = 0; i < ignition.cookingSlotsUI.Count; i++)
        {
            ignition.cookingSlotsUI[i].gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnUpdateFirePowerGaugeSlider()
    {
        _firePowerGaugeSlider.value = ignition.firePowerGauge;
    }

    public void OnSetIngredients()
    {
        ClearItems();

        foreach (var item in ignition.firewoodItemSlots)
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
        if (ignition.cookingSlotsUI.Count == 0)
        {
            for (int i = 0; i < ignition.recipeRequiredItemSlots.Length; i++)
            {
                var prefab = Managers.Resource.GetCache<GameObject>("UICookingSlot.prefab");
                GameObject itemUI = Instantiate(prefab, _content);
                var cookingSlot = itemUI.GetComponent<UICookingSlot>();

                cookingSlot.Set(ignition.recipeRequiredItemSlots[i]);
                cookingSlot.cookedFoodItemQuantity.text = "0";
                cookingSlot.index = i;
                cookingSlot.cookingLevel = _cookingLevel;

                itemUI.BindEvent((x) => DeliverFoodItemsToInventory(cookingSlot.index));

                ignition.cookingSlotsUI.Add(cookingSlot);
                ignition.cookingSlotsUI[i].gameObject.SetActive(false);
            }
        }
        else
        {
            return;
        }
    }

    private void ShowCookingSlots(int cookingLevel)
    {
        for (int i = 0; i < ignition.cookingSlotsUI.Count; i++)
        {
            if (ignition.cookingSlotsUI[i].cookingLevel == cookingLevel)
            {
                ignition.cookingSlotsUI[i].gameObject.SetActive(true);
            }
        }
    }

    public void Set(int index)
    {
        foreach (var recipe in ignition.recipes)
        {
            if (recipe.completedItemData == ignition.recipeRequiredItemSlots[index].itemData)
            {
                var cookingSlot = ignition.cookingSlotsUI[index];
                cookingSlot.Set(ignition.recipeRequiredItemSlots[index]);
                cookingSlot.SetMaxTimeTakenToCookSlider(recipe.maxTimeRequiredToCook);
                ignition.cookingSlotsUI[index].maxTimeRequiredToCook = recipe.maxTimeRequiredToCook;
            }
        }
    }

    private void OnUpdateQuantity(int index)
    {
        if (ignition.cookingSlotsUI[index] == null) return;

        var cookingSlot = ignition.cookingSlotsUI[index].GetComponent<UICookingSlot>();
        cookingSlot.Set(ignition.recipeRequiredItemSlots[index]);
        cookingSlot.CookedFoodItemQuantitySet(ignition.cookedFoodItems[index]);
    }

    public void OnUpdatedTimeTakenToCookSlider()
    {
        if (ignition.cookingSlotsUI.Count == 0) return;

        var cookingSlot = ignition.cookingSlotsUI[ignition.cookingSlotIndex].GetComponent<UICookingSlot>();
        cookingSlot.UpdatedTimeTakenToCookSlider(ignition.currentTimeRequiredToCook);
        OnUpdateQuantity(ignition.cookingSlotIndex);
    }

    private void DeliverFoodItemsToInventory(int index)
    {
        //_ignition.cookedFoodItems 에서 Inventory로 넘겨주기
        GameManager.Instance.Player.Inventory.TryAddItem(ignition.cookedFoodItems[index].itemData, ignition.cookedFoodItems[index].quantity);
        ignition.cookedFoodItems[index].SubtractQuantity(ignition.cookedFoodItems[index].quantity);
        var cookingSlot = ignition.cookingSlotsUI[index].GetComponent<UICookingSlot>();
        cookingSlot.CookedFoodItemQuantitySet(ignition.cookedFoodItems[index]);
    }

    private void ShowStoreFirewoodPopupUI(ItemSlot itemSlot)
    {
        int index = 0;

        for (int i = 0; i < ignition.firewoodItemSlots.Length; i++)
        {
            if (ignition.firewoodItemSlots[i].itemData == itemSlot.itemData)
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
        ui.SetAdvancedRecipeUIActive(ignition.cookingLevel);
    }

    private void ClearItems()
    {
        foreach (GameObject itemUI in _itemUIList)
        {
            Destroy(itemUI);
        }
    }
}
