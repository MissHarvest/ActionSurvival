using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 2024. 01. 16 Byun Jeongmin
public class Cooking : MonoBehaviour
{
    public Player Owner { get; private set; }
    private UICooking _cookingUI;


    private void Awake()
    {
        Debug.Log("Cooking Awake");
        Owner = Managers.Game.Player;
        var input = Owner.Input;
        input.InputActions.Player.Interact.started += OnCookingShowAndHide; //E키로 요리 UI 띄움
    }

    private void Start()
    {
        Debug.Log("Cooking Start");
    }

    public void ConsumeIngredients(List<RecipeSO.Ingredient> ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            ItemData requiredItemData = ingredient.item;
            int requiredCount = ingredient.quantity;

            Managers.Game.Player.Inventory.RemoveItem(requiredItemData, requiredCount);
        }
    }

    public bool CheckIngredients(List<RecipeSO.Ingredient> ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            int requiredQuantity = ingredient.quantity;
            int availableQuantity = Managers.Game.Player.Inventory.GetItemCount(ingredient.item);

            // 재료가 부족하면 false 반환
            if (availableQuantity < requiredQuantity)
            {
                return false;
            }
        }
        return true;
    }

    public void OnCookingShowAndHide(InputAction.CallbackContext context)
    {
        if (_cookingUI == null)
        {
            _cookingUI = Managers.UI.ShowPopupUI<UICooking>();
            return;
        }

        if (_cookingUI.gameObject.activeSelf)
        {
            Managers.UI.ClosePopupUI(_cookingUI);
        }
        else
        {
            Managers.UI.ShowPopupUI<UICooking>();
        }
    }
}
