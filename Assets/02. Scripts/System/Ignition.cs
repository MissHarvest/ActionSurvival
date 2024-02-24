using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Lee gyuseong 24.02.22

public class Ignition : MonoBehaviour
{
    public ItemSlot[] firewoodItemSlots = new ItemSlot[2];
    public ItemSlot[] recipeRequiredItemSlots = new ItemSlot[5];

    public int _count = 0;
    public int _maxCount;
    public int _index;
    public int _firePowerGauge;

    private bool _haveFirewood = false;
    private bool _startCooking = false;

    private DayCycle _dayCycle;

    public event Action<int> OnUpdatedCount;
    public event Action OnUpdatedUI;

    private void Awake()
    {
        GetFirewoodItems();
        _firePowerGauge = 0;
        for (int i = 0; i < recipeRequiredItemSlots.Length; i++)
        {
            recipeRequiredItemSlots[i] = new ItemSlot();
        }
    }

    private void Start()
    {
        _dayCycle = GameManager.DayCycle;
        _dayCycle.OnTimeUpdated += OnConsumeFirePowerGauge;
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

        if (_firePowerGauge == 0 && firewoodItemSlots[0].quantity == 0 && firewoodItemSlots[1].quantity == 0)
        {
            _startCooking = false;
            _haveFirewood = false;
        }
    }

    public void StartCookingButton()
    {
        _startCooking = true;
        StartAFire();
    }

    public void StopCookingButton()
    {
        _startCooking = false;
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

            OnUpdatedUI?.Invoke();
        }
    }

    public void OnStoreQuantity()
    {
        if (_count > 0)
        {
            firewoodItemSlots[_index].AddQuantity(_count);
            GameManager.Instance.Player.Inventory.TryConsumeQuantity(firewoodItemSlots[_index].itemData, _count);

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
