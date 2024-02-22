using System;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 18 Byun Jeongmin
// 제작, 요리를 포함하는 기본 클래스
public class CraftBase : MonoBehaviour
{
    protected int _count = 1;

    public event Action<int> OnCountChanged;

    public int Count
    {
        get { return _count; }
        private set { _count = value; }
    }

    public virtual void Awake()
    {
        Debug.Log($"{GetType().Name} Awake");
    }

    public virtual void Start()
    {

        Debug.Log($"{GetType().Name} Start");
    }

    public void InitializeCount()
    {
        _count = 1;
        OnCraftCountChanged();
    }

    public void OnMinusQuantity()
    {
        if (_count > 1)
        {
            _count--;
            OnCraftCountChanged();
        }
    }

    public void OnPlusQuantity()
    {
        // 한 번에 20개까지만 제작 가능
        if (_count < 20)
        {
            _count++;
            OnCraftCountChanged();
        }
    }

    private void OnCraftCountChanged()
    {
        OnCountChanged?.Invoke(_count);
    }

    public virtual bool CheckItems(List<RecipeSO.Ingredient> items)
    {
        foreach (var item in items)
        {
            int requiredQuantity = item.quantity * _count;

            // 재료 아이템이 부족하면 false 반환
            if (GameManager.Instance.Player.Inventory.GetItemCount(item.item) < requiredQuantity)
                return false;
        }
        return true;
    }

    public virtual void ConsumeItems(List<RecipeSO.Ingredient> items)
    {
        foreach (var item in items)
        {
            int requiredQuantity = item.quantity * _count;

            GameManager.Instance.Player.Inventory.TryConsumeQuantity(item.item, requiredQuantity);
        }
    }

    public virtual void AddItems(List<RecipeSO.Ingredient> items)
    {
        foreach (var item in items)
        {
            int requiredQuantity = item.quantity * _count;

            // 재료 아이템 돌려주기
            GameManager.Instance.Player.Inventory.TryAddItem(item.item, requiredQuantity);
        }
    }
}