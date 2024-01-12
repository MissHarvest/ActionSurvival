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
                Debug.Log("�κ��丮�� ���� á���ϴ�. �������� �߰��� �� �����ϴ�.");
            }
            else
            {
                // ���ۿ� �ʿ��� ������ ���� �� �κ��丮 �߰�
                inventory.AddItem(craftedItemData, 1);
                Debug.Log($"{craftedItemData.name}�� �����߾��.");

                // ���ۿ� �ʿ��� �������� �κ��丮���� �Ҹ�
                ConsumeItemsForCrafting(requiredItems);
            }
        }
        else
        {
            Debug.Log($"{craftedItemData.name}�� ���� ��ᰡ �����ؿ�.");
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

    // ���ϴ� ���� �ϳ��� Ŭ������ ���� �ɵ�
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

    public void MakeCraftingTable() // ���۴� ��ũ���ͺ� ������Ʈ �߰� �ʿ�
    {
        TryCraftItem(Managers.Resource.GetCache<ItemData>("CraftingTableItemData.data"), Managers.Data.recipeData["CraftingTable"]);
    }

    public void MakeStick()
    {
        TryCraftItem(Managers.Resource.GetCache<ItemData>("StickItemData.data"), Managers.Data.recipeData["Stick"]);
    }
}
