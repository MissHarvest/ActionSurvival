using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 2024. 01. 11 Byun Jeongmin
public class UIRecipe : UICraftBase
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
        SetAdvancedRecipeUIActive(0);
    }

    protected override List<RecipeSO> GetDataList()
    {
        return Managers.Data.recipeDataList;
    }
}
