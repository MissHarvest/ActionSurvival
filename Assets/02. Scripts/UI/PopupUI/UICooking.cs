using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 2024. 01. 16 Byun Jeongmin
public class UICooking : UICraftBase
{
    [SerializeField] private GameObject _ingredientPrefab;

    public override void Awake()
    {
        base.Awake();
        _itemPrefab = _ingredientPrefab;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SetAdvancedRecipeUIActive(0);
    }

    protected override List<RecipeSO.Ingredient> GetRequiredDataList()
    {
        return Managers.Data.cookingDataList.SelectMany(recipe => recipe.requiredItems).ToList();
    }

    protected override List<RecipeSO> GetDataList()
    {
        var sortedCookingList = Managers.Data.cookingDataList.OrderBy(recipe => recipe.recipeID).ToList();
        return sortedCookingList;
    }
}