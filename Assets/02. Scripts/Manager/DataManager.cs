using System;
using System.Collections.Generic;
using System.Linq;

// 2024. 01. 12 Byun Jeongmin
public enum ItemDataType
{
    Item,
    Tool,
    Weapon,
    Architecture,
    Equip
}

public class DataManager
{
    // 제작 레시피 스크립터블 오브젝트를 저장할 리스트
    public List<RecipeSO> recipeDataList = new List<RecipeSO>();

    // 요리 레시피 스크립터블 오브젝트를 저장할 리스트
    public List<RecipeSO> cookingDataList = new List<RecipeSO>();

    // ItemDataType에 따른 정렬 순서
    private Dictionary<Type, ItemDataType> itemTypeOrder = new Dictionary<Type, ItemDataType>
    {
        { typeof(ItemData), ItemDataType.Item },
        { typeof(ToolItemData), ItemDataType.Tool },
        { typeof(WeaponItemData), ItemDataType.Weapon },
        { typeof(ArchitectureItemData), ItemDataType.Architecture },
        { typeof(EquipItemData), ItemDataType.Equip }
    };

    // 데이터 초기화, GetCacheGroup으로 공통된 이름을 가진 데이터 한 번에 불러옴
    public void InitializeRecipeData()
    {
        recipeDataList = new (Managers.Resource.GetCacheGroup<RecipeSO>("RecipeData.data"));
        cookingDataList = new (Managers.Resource.GetCacheGroup<RecipeSO>("CookingData.data"));

        recipeDataList = recipeDataList
            .OrderBy(recipe => recipe.recipeLevel)
            .ThenBy(recipe => GetItemTypeOrder(recipe.completedItemData))
            .ToList();

        cookingDataList = cookingDataList
            .OrderBy(recipe => recipe.recipeLevel)
            .ThenBy(recipe => GetItemTypeOrder(recipe.completedItemData))
            .ToList();
    }

    // 정렬 순서 반환
    private ItemDataType GetItemTypeOrder(ItemData itemData)
    {
        return itemTypeOrder.GetValueOrDefault(itemData.GetType(), ItemDataType.Item);
    }

    public RecipeSO GetRecipeDataByItemName(string itemName)
    {
        return recipeDataList.Find(recipe => recipe.itemName == itemName);
    }
}
