using System.Collections.Generic;

// 2024. 01. 12 Byun Jeongmin
public class DataManager
{
    // 제작 레시피 스크립터블 오브젝트를 저장할 리스트
    public List<RecipeSO> recipeDataList = new List<RecipeSO>();

    // 요리 레시피 스크립터블 오브젝트를 저장할 리스트
    public List<RecipeSO> cookingDataList = new List<RecipeSO>();

    // 데이터 초기화, GetCacheGroup으로 공통된 이름을 가진 데이터 한 번에 불러옴
    public void InitializeRecipeData()
    {
        recipeDataList.AddRange(Managers.Resource.GetCacheGroup<RecipeSO>("RecipeData.data"));
        cookingDataList.AddRange(Managers.Resource.GetCacheGroup<RecipeSO>("CookingData.data"));
    }

    public RecipeSO GetRecipeDataByItemName(string itemName)
    {
        return recipeDataList.Find(recipe => recipe.itemName == itemName);
    }
}
