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

    // 이하는 따로 하나의 클래스로 빼도 될듯
    public void MakeAxe()
    {
        TryCraftItem(Managers.Resource.GetCache<ItemData>("AxeItemData.data"), Managers.Data.recipeData["Axe"]);
    }

    public void MakePickAxe()
    {
        TryCraftItem(Managers.Resource.GetCache<ItemData>("PickItemData.data"), Managers.Data.recipeData["PickAxe"]);
    }

    public void MakeSword()
    {
        TryCraftItem(Managers.Resource.GetCache<ItemData>("SwordItemData.data"), Managers.Data.recipeData["Sword"]);
    }

    public void MakeCraftingTable() // 제작대 스크립터블 오브젝트 추가 필요
    {
        TryCraftItem(Managers.Resource.GetCache<ItemData>("CraftingTableItemData.data"), Managers.Data.recipeData["CraftingTable"]);
    }

    public void MakeStick()
    {
        TryCraftItem(Managers.Resource.GetCache<ItemData>("StickItemData.data"), Managers.Data.recipeData["Stick"]);
    }
}
