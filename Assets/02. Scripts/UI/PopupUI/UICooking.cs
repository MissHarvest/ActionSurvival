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
        //ignition.recipes = _recipeOrCookingList;
        ignition.GetRecipe(_recipeOrCookingList);
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
                    _craftBase.ConsumeItems(items);
                    int totalQuantity = _craftBase.Count * SelectedRecipe.Quantity;

                    ignition.recipeRequiredItemSlots[_index].Set(completedItemData, totalQuantity);
                    _uiIIgnition.SetCookingData(_index);
                    ignition.startCooking = true;
                    if (ignition.haveFirewood)
                    {
                        ignition.StartAFire();
                    }
                    var confirm = Managers.UI.ShowPopupUI<UICraftConfirm>();
                    confirm.SetCraft($"{completedItemData.displayName} 요리 등록!");
                }
            }
            else
            {
                Managers.UI.ShowPopupUI<UIWarning>().SetWarning("요리 슬롯이 부족합니다.");
            }
            _confirm.gameObject.SetActive(false);
        }
    }

    protected override string GetConfirmationText(string displayName, int itemQuantity, int craftCount)
    {
        return $"{displayName}을(를) {itemQuantity} X {craftCount}개\n요리하시겠습니까?";
    }
}