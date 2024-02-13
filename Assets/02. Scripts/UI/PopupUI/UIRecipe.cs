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
        SetAdvancedRecipeUIActive(false);
    }

    // 고급 레시피 UI의 활성화 여부를 설정하는 메서드
    public void SetAdvancedRecipeUIActive(bool active)
    {
        foreach (var slot in _uiCraftSlots)
        {
            bool isAdvancedRecipe = GetDataList()[slot.Index].IsAdvancedRecipe;
            slot.gameObject.SetActive(active || (!isAdvancedRecipe && !active));
        }
    }

    protected override List<RecipeSO.Ingredient> GetRequiredDataList()
    {
        return (Managers.Data.recipeDataList.SelectMany(recipe => recipe.requiredItems).ToList());
    }

    protected override List<RecipeSO> GetDataList()
    {
        // recipeID로 오름차순 정렬
        var sortedRecipeList = Managers.Data.recipeDataList.OrderBy(recipe => recipe.recipeID).ToList();
        return sortedRecipeList;
    }
}
