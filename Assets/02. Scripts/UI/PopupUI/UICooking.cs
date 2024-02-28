using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static UnityEditor.Progress;
using UnityEngine;


// 2024. 01. 16 Byun Jeongmin
public class UICooking : UICraftBase
{
    public Ignition ignition;
    private UIIgnition _uiIIgnition;
    private int _index;

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SetAdvancedRecipeUIActive(0);
        ignition.recipes = _recipeOrCookingList;
    }

    private void Start()
    {
        _uiIIgnition = Managers.UI.GetPopupUI<UIIgnition>();
    }

    protected override void GetData()
    {
        _recipeOrCookingList = Managers.Data.cookingDataList;
        _craftBase = GameManager.Instance.Player.Cooking;
    }

    protected override void OnConfirmedBase()
    {
        //Ignition으로 선택된 레시피를 넘겨주기
        if (SelectedRecipe != null)
        {
            List<RecipeSO.Ingredient> items = SelectedRecipe.requiredItems;
            ItemData completedItemData = SelectedRecipe.completedItemData;

            bool checkEmptySlots = false;

            for (int i = 0; i < ignition.recipeRequiredItemSlots.Length; i++)
            {
                if (i > ignition.capacity - 1) break;
                if (ignition.recipeRequiredItemSlots[i].itemData == null && ignition.cookedFoodItemSlots[i].itemData == null)
                {
                    checkEmptySlots = true;
                    _index = i;
                    break;
                }
            }

            if (checkEmptySlots)
            {
                if (_craftBase.CheckItems(items))
                {
                    int totalQuantity = _craftBase.Count * SelectedRecipe.Quantity;

                    ignition.recipeRequiredItemSlots[_index].Set(completedItemData, totalQuantity);
                    _uiIIgnition.SetCookingData(_index);
                    ignition.startCooking = true;
                    if (ignition.haveFirewood)
                    {
                        ignition.StartAFire();
                    }
                    _confirm.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.Log("asd");
            }
        }
    }

    void Reference()
    {
        if (SelectedRecipe != null)
        {
            List<RecipeSO.Ingredient> items = SelectedRecipe.requiredItems;
            ItemData completedItemData = SelectedRecipe.completedItemData;

            // 플레이어 인벤토리에서 아이템 확인 및 소모
            if (_craftBase.CheckItems(items))
            {
                int totalQuantity = _craftBase.Count * SelectedRecipe.Quantity;
                if (GameManager.Instance.Player.Inventory.TryAddItem(completedItemData, totalQuantity))
                {
                    var confirm = Managers.UI.ShowPopupUI<UICraftConfirm>();
                    confirm.SetCraft($"{completedItemData.displayName} 제작 완료!");
                }
                else // 재료가 충분하고 소모도 했는데 아이템 들어갈 공간이 없을 경우 재료를 돌려줘야함
                {
                    _craftBase.AddItems(items);
                }
            }

            _confirm.gameObject.SetActive(false);
            _craftBase.InitializeCount();
            return;
        }
    }
}