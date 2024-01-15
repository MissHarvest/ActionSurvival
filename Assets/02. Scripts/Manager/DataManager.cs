using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 12 Byun Jeongmin
public class DataManager
{
    // ������ ������, csv�� json���� �����ϸ� �� ������
    public Dictionary<string, Dictionary<ItemData, int>> recipeData = new Dictionary<string, Dictionary<ItemData, int>>();

    public void InitializeRecipeData()
    {
        Dictionary<ItemData, int> requiredItemsForAxe = new Dictionary<ItemData, int>
        {
            { Managers.Resource.GetCache<ItemData>("LogItemData.data"), 1 },
            { Managers.Resource.GetCache<ItemData>("StoneItemData.data"), 5 },
        };
        recipeData.Add("Axe", requiredItemsForAxe);

        Dictionary<ItemData, int> requiredItemsForPickAxe = new Dictionary<ItemData, int>
        {
            { Managers.Resource.GetCache<ItemData>("LogItemData.data"), 1 },
            { Managers.Resource.GetCache<ItemData>("StoneItemData.data"), 3 },
        };
        recipeData.Add("PickAxe", requiredItemsForPickAxe);

        Dictionary<ItemData, int> requiredItemsForSword = new Dictionary<ItemData, int>
        {
            { Managers.Resource.GetCache<ItemData>("LogItemData.data"), 1 },
            { Managers.Resource.GetCache<ItemData>("StickItemData.data"), 1 },
        };
        recipeData.Add("Sword", requiredItemsForSword);

        Dictionary<ItemData, int> requiredItemsForCraftingTable = new Dictionary<ItemData, int>
        {
            { Managers.Resource.GetCache<ItemData>("LogItemData.data"), 4 },
        };
        recipeData.Add("CraftingTable", requiredItemsForCraftingTable);

        Dictionary<ItemData, int> requiredItemsForStick = new Dictionary<ItemData, int>
        {
            { Managers.Resource.GetCache<ItemData>("LogItemData.data"), 2 },
        };
        recipeData.Add("Stick", requiredItemsForStick);

        // ������ �߰�
    }
}
