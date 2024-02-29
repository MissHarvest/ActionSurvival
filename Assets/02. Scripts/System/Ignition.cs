using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Lee gyuseong 24.02.22

public class Ignition : MonoBehaviour
{
    public ItemSlot[] firewoodItemSlots = new ItemSlot[2];
    public ItemSlot[] recipeRequiredItemSlots;
    public ItemSlot[] cookedFoodItemSlots;
    public List<RecipeSO> recipes = new List<RecipeSO>();
    public int[] currentTimeRequiredToCook;

    public int capacity = 0;
    [SerializeField] private int _maxCookingSlot = 6;
    public int firewoodStoreCount = 0;
    public int maxCount;
    public int firewoodItemIndex;
    public int firePowerGauge;
    public int cookingLevel = 0;
    public int cookingSlotIndex;
    private int _maxTime;

    public bool haveFirewood = false;
    public bool startCooking = false;

    private DayCycle _dayCycle;

    public event Action<int> OnUpdatedCount;
    public event Action OnUpdateFirewoodUI;
    public event Action<int, int> OnUpdateSlider;
    public event Action<int> OnUpdateQuantity;
    public event Action OnClosePopup;

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
        recipeRequiredItemSlots = new ItemSlot[_maxCookingSlot];
        cookedFoodItemSlots = new ItemSlot[_maxCookingSlot];
        currentTimeRequiredToCook = new int[_maxCookingSlot];

        GetFirewoodItems();

        firePowerGauge = 0;

        for (int i = 0; i < recipeRequiredItemSlots.Length; i++)
        {
            recipeRequiredItemSlots[i] = new ItemSlot();
            cookedFoodItemSlots[i] = new ItemSlot();
            currentTimeRequiredToCook[i] = 0;
        }

        Load();

        GameManager.Instance.OnSaveCallback += Save;
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
                    firePowerGauge = i == 0 ? 10 : 20;
                    OnUpdateFirewoodUI?.Invoke();

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
            OnUpdateFirewoodUI?.Invoke();
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

    private void SetCookingSlotIndex()
    {
        if (recipeRequiredItemSlots[cookingSlotIndex].itemData != null)
        {
            return;
        }
        if (Array.TrueForAll(currentTimeRequiredToCook, x => x == 0))
        {
            for (int i = 0; i < recipeRequiredItemSlots.Length; i++)
            {
                if (recipeRequiredItemSlots[i].itemData != null)
                {
                    cookingSlotIndex = i;
                    break;
                }
            }
        }
    }

    public void OnStartCooking()
    {
        if (!startCooking || !haveFirewood) return;

        SetCookingSlotIndex();

        foreach (var recipe in recipes)
        {
            if (recipe.completedItemData == recipeRequiredItemSlots[cookingSlotIndex].itemData)
            {
                _maxTime = recipe.maxTimeRequiredToCook;
            }
        }

        currentTimeRequiredToCook[cookingSlotIndex] += 1;
        OnUpdateSlider?.Invoke(cookingSlotIndex, currentTimeRequiredToCook[cookingSlotIndex]);

        if (currentTimeRequiredToCook[cookingSlotIndex] >= _maxTime)
        {
            if (cookedFoodItemSlots[cookingSlotIndex].itemData == recipeRequiredItemSlots[cookingSlotIndex].itemData)
            {
                cookedFoodItemSlots[cookingSlotIndex].AddQuantity(1);
                recipeRequiredItemSlots[cookingSlotIndex].SubtractQuantity(1);

                OnUpdateQuantity?.Invoke(cookingSlotIndex);
            }

            if (cookedFoodItemSlots[cookingSlotIndex].itemData == null)
            {
                cookedFoodItemSlots[cookingSlotIndex].Set(recipeRequiredItemSlots[cookingSlotIndex].itemData, 1);
                recipeRequiredItemSlots[cookingSlotIndex].SubtractQuantity(1);

                OnUpdateQuantity?.Invoke(cookingSlotIndex);
            }

            if (Array.TrueForAll(recipeRequiredItemSlots, x => x.itemData == null))
            {
                startCooking = false;
            }

            currentTimeRequiredToCook[cookingSlotIndex] = 0;
            OnUpdateSlider?.Invoke(cookingSlotIndex, currentTimeRequiredToCook[cookingSlotIndex]);
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

            OnUpdateFirewoodUI?.Invoke();
            OnClosePopup?.Invoke();
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
            OnUpdateFirewoodUI?.Invoke();
            OnClosePopup?.Invoke();
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

    private void Load()
    {
        if (SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, $"{gameObject.name}Ignition"))
        {
            Debug.LogWarning("TryLoadJsonToObject");
            for (int i = 0; i < firewoodItemSlots.Length; i++)
            {                
                firewoodItemSlots[i].LoadData();
            }
            Debug.LogWarning("firewoodslotload");
            for (int i = 0; i < recipeRequiredItemSlots.Length; i++)
            {
                recipeRequiredItemSlots[i].LoadData();
                cookedFoodItemSlots[i].LoadData();
            }
            Debug.LogWarning("recipeRequiredItemSlotsLoad");
        }
    }

    private void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile($"{gameObject.name}Ignition", json, SaveGame.SaveType.Runtime);
    }
}