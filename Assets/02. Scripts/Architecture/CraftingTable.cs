using UnityEngine;

// 2024. 01. 12 Byun Jeongmin
public class CraftingTable : MonoBehaviour, IInteractable
{
    [SerializeField] private int _recipeLevel = 1;

    public float GetInteractTime()
    {
        throw new System.NotImplementedException();
    }

    // Level
    public void Interact(Player player)
    {
        var ui = Managers.UI.ShowPopupUI<UIRecipe>();
        ui.SetAdvancedRecipeUIActive( _recipeLevel);
        // ui.Level 넘겨주기
    }
}