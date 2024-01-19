using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 2024. 01. 17 Byun Jeongmin
public class UICookingConfirm : UIConfirmBase
{
    [SerializeField] private GameObject _ingredientPrefab;

    private void Awake()
    {
        base.Awake();
        _itemPrefab = _ingredientPrefab;
    }

    private void OnEnable()
    {
        base.OnEnable();
    }

    protected override List<RecipeSO.Ingredient> GetRequiredDataList()
    {
        return Managers.Data.cookingDataList.SelectMany(recipe => recipe.requiredItems).ToList();
    }

    protected override List<RecipeSO> GetDataList()
    {
        return Managers.Data.cookingDataList;
    }
}
