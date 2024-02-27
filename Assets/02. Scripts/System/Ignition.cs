using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Lee gyuseong 24.02.22

public class Ignition : MonoBehaviour
{
    public ItemSlot[] firewoodItemSlots = new ItemSlot[2];
    public ItemSlot[] recipeRequiredItemSlots;
    public ItemSlot[] cookedFoodItems;
    public List<RecipeSO> recipes = new List<RecipeSO>();
    public List<UICookingSlot> cookingSlotsUI = new List<UICookingSlot>();

    [SerializeField] private int _capacity = 0;
    public int firewoodStoreCount = 0;
    public int maxCount;
    public int firewoodItemIndex;
    public int firePowerGauge;
    public int currentTimeRequiredToCook;
    public int cookingLevel = 0;
    public int cookingSlotIndex;

    public bool haveFirewood = false;
    public bool startCooking = false;
    public bool cookingDone = false;

    private DayCycle _dayCycle;

    public event Action<int> OnUpdatedCount;
    public event Action OnUpdatedUI;
    public event Action<int> OnUpdateQuantity;
    public void StartCookingButton()
    {
        startCooking = true;
        StartAFire();
    }

    public void StopCookingButton()
    {
        startCooking = false;
    }

    private void Awake()
    {
        recipeRequiredItemSlots = new ItemSlot[_capacity];
        cookedFoodItems = new ItemSlot[_capacity];

        GetFirewoodItems();

        currentTimeRequiredToCook = 0;
        firePowerGauge = 0;

        for (int i = 0; i < recipeRequiredItemSlots.Length; i++)
        {
            recipeRequiredItemSlots[i] = new ItemSlot();
            cookedFoodItems[i] = new ItemSlot();
        }
    }

    private void Start()
    {
        _dayCycle = GameManager.DayCycle;
        _dayCycle.OnTimeUpdated += OnConsumeFirePowerGauge;
        _dayCycle.OnTimeUpdated += OnStartCooking;
    }

    public void StartAFire()
    {
        if (startCooking && firePowerGauge < 1)
        {
            for (int i = 0; i < firewoodItemSlots.Length; i++)
            {
                if (firewoodItemSlots[i].quantity > 0)
                {
                    firewoodItemSlots[i].SubtractFirewoodItemQuantity(1);
                    firePowerGauge = i == 0 ? 2 : 3;
                    OnUpdatedUI?.Invoke();

                    haveFirewood = true;
                    return;
                }
            }
        }
    }

    public void OnConsumeFirePowerGauge()
    {
        //daycycle에서 이벤트 호출되면 일정 수치 만큼 깎는다.
        if (haveFirewood)
        {
            firePowerGauge -= 1;
        }
        else
        {
            return;
        }

        if (firePowerGauge < 1)
        {
            StartAFire();
        }

        if (Array.TrueForAll(recipeRequiredItemSlots, x => x.itemData == null))
        {
            startCooking = false;
        }

        if (firePowerGauge == 0 && firewoodItemSlots[0].quantity == 0 && firewoodItemSlots[1].quantity == 0)
        {
            //_startCooking = false;
            haveFirewood = false;
        }
    }

    public void OnStartCooking()
    {
        if (!startCooking || !haveFirewood) return;

        for (int i = 0; i < recipeRequiredItemSlots.Length; i++)
        {
            if (recipeRequiredItemSlots[i].itemData != null)
            {
                cookingSlotsUI[i].GetComponent<UICookingSlot>();
                cookingSlotIndex = i;
                break;
            }
        }

        currentTimeRequiredToCook += 1;
        cookingSlotsUI[cookingSlotIndex].UpdatedTimeTakenToCookSlider(currentTimeRequiredToCook);

        if (currentTimeRequiredToCook >= cookingSlotsUI[cookingSlotIndex].maxTimeRequiredToCook)
        {
            if (cookedFoodItems[cookingSlotIndex].itemData == recipeRequiredItemSlots[cookingSlotIndex].itemData)
            {
                cookedFoodItems[cookingSlotIndex].AddQuantity(1);
                recipeRequiredItemSlots[cookingSlotIndex].SubtractQuantity(1);
                if (recipeRequiredItemSlots[cookingSlotIndex].itemData == null) cookingDone = true;
                OnUpdateQuantity?.Invoke(cookingSlotIndex);
            }

            if (cookedFoodItems[cookingSlotIndex].itemData == null)
            {
                cookedFoodItems[cookingSlotIndex].Set(recipeRequiredItemSlots[cookingSlotIndex].itemData, 1);
                recipeRequiredItemSlots[cookingSlotIndex].SubtractQuantity(1);
                if (recipeRequiredItemSlots[cookingSlotIndex].itemData == null) cookingDone = true;
                OnUpdateQuantity?.Invoke(cookingSlotIndex);
            }

            if (Array.TrueForAll(recipeRequiredItemSlots, x => x.itemData == null))
            {
                startCooking = false;
            }

            currentTimeRequiredToCook = 0;
            cookingSlotsUI[cookingSlotIndex].UpdatedTimeTakenToCookSlider(currentTimeRequiredToCook);
        }
    }

    private void GetFirewoodItems()
    {
        var itemData = Managers.Resource.GetCache<ItemData>("BranchItemData.data");
        firewoodItemSlots[0].Set(itemData, 0);
        itemData = Managers.Resource.GetCache<ItemData>("LogItemData.data");
        firewoodItemSlots[1].Set(itemData, 0);
    }

    public void OnTakeoutQuantity()
    {
        var itemSlots = firewoodItemSlots[firewoodItemIndex];

        if (itemSlots.quantity > 0)
        {
            GameManager.Instance.Player.Inventory.TryAddItem(itemSlots.itemData, itemSlots.quantity);
            firewoodItemSlots[firewoodItemIndex].SubtractFirewoodItemQuantity(itemSlots.quantity);

            if (firewoodItemSlots[0].quantity == 0 && firewoodItemSlots[1].quantity == 0)
            {
                haveFirewood = false;
            }

            OnUpdatedUI?.Invoke();
        }
    }

    public void OnStoreQuantity()
    {
        if (firewoodStoreCount > 0)
        {
            firewoodItemSlots[firewoodItemIndex].AddQuantity(firewoodStoreCount);
            GameManager.Instance.Player.Inventory.TryConsumeQuantity(firewoodItemSlots[firewoodItemIndex].itemData, firewoodStoreCount);
            haveFirewood = true;
            StartAFire();
            OnUpdatedUI?.Invoke();
        }
    }

    public void OnMinusQuantity()
    {
        if (firewoodStoreCount > 0)
        {
            firewoodStoreCount--;
            UpdateCountUI();
        }
    }

    public void OnPlusQuantity()
    {
        if (firewoodStoreCount < maxCount)
        {
            firewoodStoreCount++;
            UpdateCountUI();
        }
    }

    private void UpdateCountUI()
    {
        OnUpdatedCount?.Invoke(firewoodStoreCount);
    }
}
