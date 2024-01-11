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
        inventory = Managers.Game.Player.GetComponentInChildren<InventorySystem>();
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

                // �κ��丮 UI ������Ʈ �ʿ�
            }
        }
        else
        {
            Debug.Log($"{craftedItemData.name} ��ᰡ �����ؿ�.");
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

            //�����Ϸ��� �Ʒ��� ���� ȣ��
            MakePickAxe();
        }
    }


    private void MakeAxe()
    {
        Dictionary<ItemData, int> requiredItemsForAxe = new Dictionary<ItemData, int>
            {
                { Resources.Load<ScriptableObject>("SO/LogItemData") as ItemData, 1 },
                { Resources.Load<ScriptableObject>("SO/StoneItemData") as ItemData, 5 }
            };
        TryCraftItem(Resources.Load<ScriptableObject>("SO/AxeItemData") as ItemData, requiredItemsForAxe);
    }

    private void MakePickAxe()
    {
        Dictionary<ItemData, int> requiredItemsForPickAxe = new Dictionary<ItemData, int>
            {
                { Resources.Load<ScriptableObject>("SO/LogItemData") as ItemData, 1 },
                { Resources.Load<ScriptableObject>("SO/StoneItemData") as ItemData, 3 }
            };
        TryCraftItem(Resources.Load<ScriptableObject>("SO/PickItemData") as ItemData, requiredItemsForPickAxe);
    }

    private void MakeSword() // ���� ����� �ʿ�
    {
        Dictionary<ItemData, int> requiredItemsForSword = new Dictionary<ItemData, int>
            {
                { Resources.Load<ScriptableObject>("SO/LogItemData") as ItemData, 1 },
                { Resources.Load<ScriptableObject>("SO/StoneItemData") as ItemData, 1 }
            };
        TryCraftItem(Resources.Load<ScriptableObject>("SO/SwordItemData") as ItemData, requiredItemsForSword);
    }

    private void MakeCraftingTable() // ���۴� ��ũ���ͺ� ������Ʈ �߰� �ʿ�
    {
        Dictionary<ItemData, int> requiredItemsForSword = new Dictionary<ItemData, int>
            {
                { Resources.Load<ScriptableObject>("SO/LogItemData") as ItemData, 4 }
            };
        TryCraftItem(Resources.Load<ScriptableObject>("SO/CraftingTableData") as ItemData, requiredItemsForSword);
    }
}
