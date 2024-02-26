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
    public List<UICookingSlot> _cookingSlotsUI = new List<UICookingSlot>();

    [SerializeField] private int _capacity = 0;
    public int _count = 0;
    public int _maxCount;
    public int _index;
    public int _firePowerGauge;
    public int _currentTimeRequiredToCook;

    public bool _haveFirewood = false;
    public bool _startCooking = false;
    public bool cookingDone = false;

    private DayCycle _dayCycle;

    public event Action<int> OnUpdatedCount;
    public event Action OnUpdatedUI;
    public event Action<int> OnUpdateQuantity;
    public void StartCookingButton()
    {
        _startCooking = true;
        StartAFire();
    }

    public void StopCookingButton()
    {
        _startCooking = false;
    }

    private void Awake()
    {
        recipeRequiredItemSlots = new ItemSlot[_capacity];
        cookedFoodItems = new ItemSlot[_capacity];

        GetFirewoodItems();

        _currentTimeRequiredToCook = 0;
        _firePowerGauge = 0;

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

    private void StartAFire()
    {        
        if (_startCooking)
        {
            for (int i = 0; i < firewoodItemSlots.Length; i++)
            {
                if (firewoodItemSlots[i].quantity > 0)
                {
                    firewoodItemSlots[i].SubtractFirewoodItemQuantity(1);
                    _firePowerGauge = i == 0 ? 2 : 3;
                    OnUpdatedUI?.Invoke();

                    _haveFirewood = true;
                    return;
                }
            }            
        }       
    }

    public void OnConsumeFirePowerGauge()
    {
        //daycycle에서 이벤트 호출되면 일정 수치 만큼 깎는다.
        if (_haveFirewood)
        {
            _firePowerGauge -= 1;
        }

        if (_firePowerGauge < 1)
        {
            StartAFire();
        }

        for (int i = 0; i < recipeRequiredItemSlots.Length; i++)
        {
            if (recipeRequiredItemSlots[i].itemData != null)
            {
                _startCooking = true;
                break;
            }
            else
            {
                _startCooking = false;
            }
        }

        if (_firePowerGauge == 0 && firewoodItemSlots[0].quantity == 0 && firewoodItemSlots[1].quantity == 0)
        {
            //_startCooking = false;
            _haveFirewood = false;
        }
    }

    public void OnStartCooking()
    {
        if (!_startCooking) return;

        for (int i = 0; i < _cookingSlotsUI.Count; i++)
        {
            if (_cookingSlotsUI[i] != null)
            {
                _cookingSlotsUI[i].GetComponent<UICookingSlot>();
                break;
            }
        }
        

        _currentTimeRequiredToCook += 1;
        _cookingSlotsUI[0].UpdatedTimeTakenToCookSlider(_currentTimeRequiredToCook);//

        if (_currentTimeRequiredToCook >= _cookingSlotsUI[0]._maxTimeRequiredToCook)//
        {
            for (int i = 0; i < cookedFoodItems.Length; i++)
            {
                if (cookedFoodItems[i].itemData == recipeRequiredItemSlots[i].itemData)
                {
                    cookedFoodItems[i].AddQuantity(1);
                    recipeRequiredItemSlots[i].SubtractQuantity(1);
                    if (recipeRequiredItemSlots[i].itemData == null) cookingDone = true;
                    OnUpdateQuantity?.Invoke(i);
                    break;
                }

                if (cookedFoodItems[i].itemData == null)
                {
                    cookedFoodItems[i].Set(recipeRequiredItemSlots[i].itemData, 1);
                    recipeRequiredItemSlots[i].SubtractQuantity(1);
                    if (recipeRequiredItemSlots[i].itemData == null) cookingDone = true;
                    OnUpdateQuantity?.Invoke(i);
                    break;
                }
            }            
            _currentTimeRequiredToCook = 0;
            _cookingSlotsUI[0].UpdatedTimeTakenToCookSlider(_currentTimeRequiredToCook);
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
        var itemSlots = firewoodItemSlots[_index];

        if (itemSlots.quantity > 0)
        {
            GameManager.Instance.Player.Inventory.TryAddItem(itemSlots.itemData, itemSlots.quantity);
            firewoodItemSlots[_index].SubtractFirewoodItemQuantity(itemSlots.quantity);

            if (firewoodItemSlots[0].itemData == null && firewoodItemSlots[1].itemData == null)
            {
                _haveFirewood = false;
            }

            OnUpdatedUI?.Invoke();
        }
    }

    public void OnStoreQuantity()
    {
        if (_count > 0)
        {
            firewoodItemSlots[_index].AddQuantity(_count);
            GameManager.Instance.Player.Inventory.TryConsumeQuantity(firewoodItemSlots[_index].itemData, _count);
            _haveFirewood = true;

            OnUpdatedUI?.Invoke();
        }
    }

    public void OnMinusQuantity()
    {
        if (_count > 0)
        {
            _count--;
            UpdateCountUI();
        }
    }

    public void OnPlusQuantity()
    {
        if (_count < _maxCount)
        {
            _count++;
            UpdateCountUI();
        }
    }

    private void UpdateCountUI()
    {
        OnUpdatedCount?.Invoke(_count);
    }
}
