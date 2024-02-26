using System.Collections.Generic;
using UnityEngine;


// 2024. 01. 16 Byun Jeongmin
[CreateAssetMenu(fileName = "RecipeSO", menuName = "ScriptableObjects/RecipeSO", order = 1)]
public class RecipeSO : ScriptableObject
{
    public string itemName;
    public ItemData completedItemData;

    public int recipeLevel = 1;

    public int Quantity = 1; // 제작 시 얻을 수 있는 완성템 개수, 기본값=1
    public int maxTimeRequiredToCook;

    //완성 제작템 SO 추가
    public List<Ingredient> requiredItems = new List<Ingredient>();

    [System.Serializable]
    public class Ingredient
    {
        public ItemData item;
        public int quantity;
    }
}
