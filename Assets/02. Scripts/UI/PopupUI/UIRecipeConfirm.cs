using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 2024. 01. 11 Byun Jeongmin
public class UIRecipeConfirm : UIConfirmBase
{
    [SerializeField] private GameObject _materialPrefab;

    public override void Awake()
    {
        base.Awake();
        _itemPrefab = _materialPrefab;
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    protected override List<RecipeSO.Ingredient> GetRequiredDataList()
    {
        return (Managers.Data.recipeDataList.SelectMany(recipe => recipe.requiredItems).ToList());
    }

    protected override List<RecipeSO> GetDataList()
    {
        return (Managers.Data.recipeDataList);
    }
}
