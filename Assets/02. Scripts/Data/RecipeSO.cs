using System.Collections.Generic;
using UnityEngine;


// 2024. 01. 16 Byun Jeongmin
[CreateAssetMenu(fileName = "RecipeSO", menuName = "ScriptableObjects/RecipeSO", order = 1)]
public class RecipeSO : ScriptableObject
{
    public string itemName;
    public ItemData completedItemData;
    //완성 제작템 SO 추가
    public List<Ingredient> requiredItems = new List<Ingredient>();

    [System.Serializable]
    public class Ingredient
    {
        public ItemData item;
        public int quantity;
    }
}
