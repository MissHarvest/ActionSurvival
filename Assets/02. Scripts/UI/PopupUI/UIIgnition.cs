using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.Port;
using static UnityEditor.Progress;
//Lee gyuseong 24.02.22

public class UIIgnition : UIPopup
{
    private enum GameObjects
    {
        Block,
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
    public Ignition ignition;

    [SerializeField] private int _cookingLevel;
    [SerializeField] private int _capacity;

    private List<GameObject> _firewoodSlots = new List<GameObject>();
    public List<UICookingSlot> cookingSlots = new List<UICookingSlot>();

    public override void Initialize()
    {
        base.Initialize();

        Bind<GameObject>(typeof(GameObjects));
        Bind<UIStoreFirewoodHelper>(typeof(Helper));

        Get<GameObject>((int)GameObjects.Block).BindEvent((x) =>
        {
            Managers.UI.ClosePopupUI(this);
        });

        _functionsUseFireSlotButton = Get<GameObject>((int)GameObjects.UIFunctionsUseFireSlot);

        _functionsUseFireSlotButton.BindEvent((x) => { ShowCookingUIPopup(); });
        _firePowerGaugeSlider = GetComponentInChildren<Slider>();

        if (ignition != null)
        {
            _cookingLevel = ignition.cookingLevel;
            _capacity = ignition.capacity;
        }
    }

    private void OnEnable()
    {
        Initialize();

        _firewoodItems = Get<GameObject>((int)GameObjects.FirewoodItems).transform;
        _content = Get<GameObject>((int)GameObjects.Content).transform;
        _firewoodHelper = Get<UIStoreFirewoodHelper>((int)Helper.UIStoreFirewoodHelper);


        if (ignition != null)
        {
            _firewoodHelper.ignition = ignition;
            ignition.OnUpdateFirewoodUI += OnUpdateFirePowerGaugeSlider;
            ignition.OnUpdateFirewoodUI += OnUpdateFirewoodItemQuantity;
            ignition.OnUpdateQuantity += OnUpdateQuantity;
            ignition.OnUpdateSlider += OnUpdatedTimeTakenToCookSlider;
            CreatCookingSlot();
            CreatFirewoodSlot();
            ShowCookingSlots();
            OnUpdateFirewoodItemQuantity();
            OnUpdateFirePowerGaugeSlider();

            for (int i = 0; i < cookingSlots.Count; i++)
            {
                OnUpdatedTimeTakenToCookSlider(i, ignition.currentTimeRequiredToCook[i]);
            }

            UpdateQuantityOnEnable();
        }
    }

    private void OnDisable()
    {
        if (ignition != null)
        {
            ignition.OnUpdateFirewoodUI -= OnUpdateFirePowerGaugeSlider;
            ignition.OnUpdateFirewoodUI -= OnUpdateFirewoodItemQuantity;
            ignition.OnUpdateQuantity -= OnUpdateQuantity;
            ignition.OnUpdateSlider -= OnUpdatedTimeTakenToCookSlider;

            for (int i = 0; i < cookingSlots.Count; i++)
            {
                cookingSlots[i].SetDisableIcon();
                cookingSlots[i].gameObject.SetActive(false);
            }
            ignition = null;
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

    public void CreatFirewoodSlot()
    {
        if (_firewoodSlots.Count == 0)
        {
            foreach (var item in ignition.firewoodItemSlots)
            {
                var prefab = Managers.Resource.GetCache<GameObject>("UIFirewoodSlot.prefab");
                GameObject itemUI = Instantiate(prefab, _firewoodItems);
                Image itemIcon = itemUI.transform.Find("Icon").GetComponent<Image>();
                TextMeshProUGUI itemQuantity = itemUI.GetComponentInChildren<TextMeshProUGUI>();

                itemIcon.sprite = item.itemData.iconSprite;
                itemQuantity.text = item.quantity.ToString();

                itemUI.BindEvent((x) => { ShowStoreFirewoodPopupUI(item); });

                _firewoodSlots.Add(itemUI);
            }
        }
        else
        {
            return;
        }
    }

    private void OnUpdateFirewoodItemQuantity()
    {
        for (int i = 0; i < _firewoodSlots.Count; i++)
        {
            TextMeshProUGUI itemQuantity = _firewoodSlots[i].GetComponentInChildren<TextMeshProUGUI>();
            itemQuantity.text = ignition.firewoodItemSlots[i].quantity.ToString();
        }
    }

    public void CreatCookingSlot()
    {
        if (_firewoodSlots.Count == 0)
        {
            for (int i = 0; i < ignition.recipeRequiredItemSlots.Length; i++)
            {
                var prefab = Managers.Resource.GetCache<GameObject>("UICookingSlot.prefab");
                GameObject itemUI = Instantiate(prefab, _content);
                var cookingSlot = itemUI.GetComponent<UICookingSlot>();
                cookingSlot.Set(ignition.recipeRequiredItemSlots[i]);
                cookingSlot.SetCookedFoodItemQuantity(ignition.cookedFoodItemSlots[i]);
                cookingSlot.index = i;

                itemUI.BindEvent((x) => DeliverFoodItemsToInventory(cookingSlot.index));

                cookingSlots.Add(cookingSlot);
                cookingSlots[i].gameObject.SetActive(false);
            }
        }
        else
        {
            return;
        }
    }

    private void ShowCookingSlots()
    {
        for (int i = 0; i < _capacity; i++)
        {
            cookingSlots[i].gameObject.SetActive(true);
        }
    }

    public void SetCookingData(int index)
    {
        foreach (var recipe in ignition.recipes)
        {
            if (recipe.completedItemData == ignition.recipeRequiredItemSlots[index].itemData)
            {
                var cookingSlot = cookingSlots[index];
                cookingSlot.Set(ignition.recipeRequiredItemSlots[index]);
                cookingSlot.SetCookedFoodItemQuantity(ignition.cookedFoodItemSlots[index]);
                cookingSlot.SetMaxTimeTakenToCookSlider(recipe.maxTimeRequiredToCook);
                cookingSlot.SetText("요리 대기");
            }
        }
    }

    private void UpdateQuantityOnEnable()
    {
        for (int i = 0; i < cookingSlots.Count; i++)
        {
            OnUpdateQuantity(i);
        }
    }

    private void OnUpdateQuantity(int index)
    {
        if (ignition.recipeRequiredItemSlots[index].itemData == null && ignition.cookedFoodItemSlots[index].itemData != null)
        {
            cookingSlots[index].Set(ignition.recipeRequiredItemSlots[index]);
            cookingSlots[index].SetCookedFoodIcon(ignition.cookedFoodItemSlots[index]);
            cookingSlots[index].SetText("요리 완료");
        }
        else
        {
            cookingSlots[index].Set(ignition.recipeRequiredItemSlots[index]);
        }

        cookingSlots[index].SetCookedFoodItemQuantity(ignition.cookedFoodItemSlots[index]);
    }

    public void OnUpdatedTimeTakenToCookSlider(int index, int currentTime)
    {
        cookingSlots[index].UpdatedTimeTakenToCookSlider(currentTime);
        if (currentTime > 0)
        {
            cookingSlots[index].SetText("요리 중...");
        }
    }

    private void DeliverFoodItemsToInventory(int index)
    {
        if (ignition.cookedFoodItemSlots[index].itemData != null)
        {
            GameManager.Instance.Player.Inventory.TryAddItem(ignition.cookedFoodItemSlots[index].itemData, ignition.cookedFoodItemSlots[index].quantity);
            ignition.cookedFoodItemSlots[index].SubtractQuantity(ignition.cookedFoodItemSlots[index].quantity);
            OnUpdateQuantity(index);

            if (ignition.recipeRequiredItemSlots[index].itemData == null)
            {
                cookingSlots[index].SetText("요리 슬롯");
            }
            else
            {
                cookingSlots[index].SetText("요리 대기");
            }
        }
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
        ui.SetAdvancedRecipeUIActive(_cookingLevel);
    }
}
