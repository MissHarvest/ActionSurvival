using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using static UnityEditor.Progress;


// 2024. 01. 16 Byun Jeongmin
public class UICooking : UICraftBase
{
    private Ignition _ignition;
    private UIIgnition _uiIIgnition;

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SetAdvancedRecipeUIActive(0);
        _ignition = GameManager.Instance.Player.Ignition;
        _ignition.recipes = _recipeOrCookingList;
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
            foreach (var slot in _ignition.recipeRequiredItemSlots)
            {
                if (slot.itemData == null)
                {
                    checkEmptySlots = true;
                }
            }

            if (checkEmptySlots)
            {
                if (_craftBase.CheckItems(items))
                {
                    int totalQuantity = _craftBase.Count * SelectedRecipe.Quantity;
                    for (int index = 0; index < _ignition.recipeRequiredItemSlots.Length; index++)
                    {
                        if (_ignition.recipeRequiredItemSlots[index].itemData == null)
                        {
                            _ignition.recipeRequiredItemSlots[index].Set(completedItemData, totalQuantity);
                            _uiIIgnition.Set(index);
                            //_ignition._startCooking = true;
                            _confirm.gameObject.SetActive(false);
                            return;
                        }
                    }
                }
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