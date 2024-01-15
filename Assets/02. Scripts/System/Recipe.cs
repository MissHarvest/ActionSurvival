using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static DataManager;

// 2024. 01. 11 Byun Jeongmin
public class Recipe : MonoBehaviour
{
    public Player Owner { get; private set; }
    private InventorySystem _inventory;
    private UIRecipe _recipeUI;

    private void Awake()
    {
        Debug.Log("Recipe Awake");
        Owner = Managers.Game.Player;
        var input = Owner.Input;
        input.InputActions.Player.Recipe.started += OnRecipeShowAndHide;
    }

    private void Start()
    {
        Debug.Log("Recipe Start");
        _inventory = Managers.Game.Player.Inventory;
    }

    private void Update()
    {
        //CountItems();
    }

    private void TryCraftItem(ItemData craftedItemData, Dictionary<ItemData, int> requiredItems)
    {
        if (CanCraftItem(requiredItems))
        {
            if (_inventory.IsFull())
            {
                Debug.Log("인벤토리가 가득 찼습니다. 아이템을 추가할 수 없습니다.");
            }
            else
            {
                // 제작에 필요한 아이템 생성 및 인벤토리 추가
                _inventory.AddItem(craftedItemData, 1);
                Debug.Log($"{craftedItemData.name}을 제작했어요.");

                // 제작에 필요한 아이템을 인벤토리에서 소모
                ConsumeItemsForCrafting(requiredItems);
            }
        }
        else
        {
            Debug.Log($"{craftedItemData.name}을 만들 재료가 부족해요.");
        }
    }

    private bool CanCraftItem(Dictionary<ItemData, int> requiredItems)
    {
        foreach (var requiredItem in requiredItems)
        {
            ItemData requiredItemData = requiredItem.Key;
            int requiredCount = requiredItem.Value;

            int availableCount = _inventory.GetItemCount(requiredItemData);

            if (availableCount < requiredCount)
            {
                return false;
            }
        }

        return true;
    }

    private void ConsumeItemsForCrafting(Dictionary<ItemData, int> requiredItems)
    {
        foreach (var requiredItem in requiredItems)
        {
            ItemData requiredItemData = requiredItem.Key;
            int requiredCount = requiredItem.Value;

            _inventory.RemoveItem(requiredItemData, requiredCount);
        }
    }

    private void OnRecipeShowAndHide(InputAction.CallbackContext context)
    {
        if (_recipeUI == null)
        {
            _recipeUI = Managers.UI.ShowPopupUI<UIRecipe>();
            return;
        }

        if (_recipeUI.gameObject.activeSelf)
        {
            Managers.UI.ClosePopupUI(_recipeUI);
        }
        else
        {
            Managers.UI.ShowPopupUI<UIRecipe>();
        }
    }

    //private void MakeItem(string itemName)
    //{
    //    TryCraftItem(Managers.Resource.GetCache<ItemData>($"{itemName}ItemData.data"), Managers.Data.recipeData[itemName]);
    //}

    private void MakeItem(string itemName)
    {
        DataManager.Recipe recipe = Managers.Data.recipeDataList.Find(r => r.itemName == itemName);

        if (recipe != null)
        {
            TryCraftItem(Managers.Resource.GetCache<ItemData>($"{itemName}ItemData.data"), ToDictionary(recipe.requiredItems));
        }
        else
        {
            Debug.LogError($"레시피를 찾을 수 없습니다: {itemName}");
        }
    }

    public Dictionary<ItemData, int> ToDictionary(List<Ingredient> ingredients)
    {
        Dictionary<ItemData, int> result = new Dictionary<ItemData, int>();
        foreach (Ingredient ingredient in ingredients)
        {
            result[ingredient.item] = ingredient.quantity;
        }
        return result;
    }


    public void MakeAxe() => MakeItem("Axe");
    public void MakePickAxe() => MakeItem("PickAxe");
    public void MakeSword() => MakeItem("Sword");
    public void MakeCraftingTable() => MakeItem("CraftingTable");
    public void MakeStick() => MakeItem("Stick");


    public void MakeGreatsword() => MakeItem("Greatsword");
}
