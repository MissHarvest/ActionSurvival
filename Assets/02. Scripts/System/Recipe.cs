using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 2024. 01. 11 Byun Jeongmin
public class Recipe : MonoBehaviour
{
    public Player Owner { get; private set; }
    private InventorySystem inventory;
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
        inventory = Managers.Game.Player.Inventory;
    }

    private void Update()
    {
        //CountItems();
    }

    private void TryCraftItem(ItemData craftedItemData, Dictionary<ItemData, int> requiredItems)
    {
        if (CanCraftItem(requiredItems))
        {
            if (inventory.IsFull())
            {
                Debug.Log("인벤토리가 가득 찼습니다. 아이템을 추가할 수 없습니다.");
            }
            else
            {
                // 제작에 필요한 아이템 생성 및 인벤토리 추가
                inventory.AddItem(craftedItemData, 1);
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

            int availableCount = inventory.GetItemCount(requiredItemData);

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

            inventory.RemoveItem(requiredItemData, requiredCount);
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

    private void MakeItem(string itemName)
    {
        TryCraftItem(Managers.Resource.GetCache<ItemData>($"{itemName}ItemData.data"), Managers.Data.recipeData[itemName]);
    }

    public void MakeAxe() => MakeItem("Axe");
    public void MakePickAxe() => MakeItem("PickAxe");
    public void MakeSword() => MakeItem("Sword");
    public void MakeCraftingTable() => MakeItem("CraftingTable");
    public void MakeStick() => MakeItem("Stick");
}
