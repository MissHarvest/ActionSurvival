using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 2024. 01. 17 Byun Jeongmin
public class UICookingConfirm : UIConfirmBase
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
