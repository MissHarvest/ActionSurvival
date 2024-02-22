using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 18 Byun Jeongmin
// 제작, 요리를 포함하는 기본 클래스
public class CraftBase : MonoBehaviour
{
    protected UICraftBase _uiCraftBase;
    protected int _count = 1;

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
    }

    public void OnMinusQuantity()
    {
        if (_count > 1)
        {
            _count--;
            _uiCraftBase.UpdateCraftUI();
        }
    }

    public void OnPlusQuantity()
    {
        // 한 번에 20개까지만 제작 가능
        if (_count < 20)
        {
            _count++;
            _uiCraftBase.UpdateCraftUI();
        }
    }

    public virtual bool CheckItems(List<RecipeSO.Ingredient> items)
    {
        foreach (var item in items)
        {
            int requiredQuantity = item.quantity * _count;

            // 재료 아이템이 부족하면 false 반환
            if (!Managers.Game.Player.Inventory.TryConsumeQuantity(item.item, requiredQuantity))
            {
                // false 되기 전까지 소모한 재료들을 돌려줌
                foreach (var consumedItem in items)
                {
                    if (consumedItem == item)
                        break;

                    int consumedQuantity = consumedItem.quantity * _count;
                    Managers.Game.Player.Inventory.TryAddItem(consumedItem.item, consumedQuantity);
                }
                return false;
            }
        }
        return true;
    }

    public virtual void AddItems(List<RecipeSO.Ingredient> items)
    {
        foreach (var item in items)
        {
            int requiredQuantity = item.quantity * _count;

            // 재료 아이템 돌려주기
            Managers.Game.Player.Inventory.TryAddItem(item.item, requiredQuantity);
        }
    }
}