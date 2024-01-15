using System.Collections.Generic;

// 2024. 01. 12 Byun Jeongmin
public class DataManager
{
    // serialize 가능한 클래스
    // 재료 이름, 개수 가진 2차원 배열
    // 레시피 데이터, csv나 json으로 관리하면 더 좋을듯


    [System.Serializable]
    public class Recipe
    {
        public string itemName;
        public List<Ingredient> requiredItems = new List<Ingredient>();
    }

    [System.Serializable]
    public class Ingredient
    {
        public ItemData item;
        public int quantity;
    }
    // 로직을 레시피 생성자 안에 넣어서 처리
    public List<Recipe> recipeDataList = new List<Recipe>();

    //스크립터블 오브젝트 안에 스크립터블 오브젝트 넣기
    public void InitializeRecipeData()
    {
        recipeDataList.Add(new Recipe
        {
            itemName = "Axe",
            requiredItems = new List<Ingredient>
            {
                new Ingredient { item = Managers.Resource.GetCache<ItemData>("LogItemData.data"), quantity = 1 },
                new Ingredient { item = Managers.Resource.GetCache<ItemData>("StoneItemData.data"), quantity = 5 }
            }
        });

        recipeDataList.Add(new Recipe
        {
            itemName = "PickAxe",
            requiredItems = new List<Ingredient>
            {
                new Ingredient { item = Managers.Resource.GetCache<ItemData>("LogItemData.data"), quantity = 1 },
                new Ingredient { item = Managers.Resource.GetCache<ItemData>("StoneItemData.data"), quantity = 3 }
            }
        });

        recipeDataList.Add(new Recipe
        {
            itemName = "Sword",
            requiredItems = new List<Ingredient>
            {
                new Ingredient { item = Managers.Resource.GetCache<ItemData>("LogItemData.data"), quantity = 1 },
                new Ingredient { item = Managers.Resource.GetCache<ItemData>("StickItemData.data"), quantity = 1 }
            }
        });

        recipeDataList.Add(new Recipe
        {
            itemName = "CraftingTable",
            requiredItems = new List<Ingredient>
            {
                new Ingredient { item = Managers.Resource.GetCache<ItemData>("LogItemData.data"), quantity = 4 }
            }
        });

        recipeDataList.Add(new Recipe
        {
            itemName = "Stick",
            requiredItems = new List<Ingredient>
            {
                new Ingredient { item = Managers.Resource.GetCache<ItemData>("LogItemData.data"), quantity = 2 }
            }
        });

        // 고급 레시피
        recipeDataList.Add(new Recipe
        {
            itemName = "Greatsword",
            requiredItems = new List<Ingredient>
            {
                new Ingredient { item = Managers.Resource.GetCache<ItemData>("LowStoneItemData.data"), quantity = 1 },
                new Ingredient { item = Managers.Resource.GetCache<ItemData>("StoneItemData.data"), quantity = 10 }
            }
        });
    }
}
