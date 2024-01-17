using System.Collections.Generic;
using UnityEngine;


// 2024. 01. 16 Byun Jeongmin
[CreateAssetMenu(fileName = "RecipeSO", menuName = "ScriptableObjects/RecipeSO", order = 1)]
public class RecipeSO : ScriptableObject
{
    public string itemName;
    public List<Ingredient> requiredItems = new List<Ingredient>();

    [System.Serializable]
    public class Ingredient
    {
        public ItemData item;
        public int quantity;
    }
}
