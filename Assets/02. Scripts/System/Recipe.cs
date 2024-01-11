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
                Debug.Log("인벤토리가 가득 찼습니다. 아이템을 추가할 수 없습니다.");
            }
            else
            {
                // 제작에 필요한 아이템 생성 및 인벤토리 추가
                inventory.AddItem(craftedItemData, 1);
                Debug.Log($"{craftedItemData.name}을 제작했어요.");

                // 제작에 필요한 아이템을 인벤토리에서 소모
                ConsumeItemsForCrafting(requiredItems);

                // 인벤토리 UI 업데이트 필요
            }
        }
        else
        {
            Debug.Log($"{craftedItemData.name} 재료가 부족해요.");
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

            //제작하려면 아래와 같이 호출
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

    private void MakeSword() // 나무 막대기 필요
    {
        Dictionary<ItemData, int> requiredItemsForSword = new Dictionary<ItemData, int>
            {
                { Resources.Load<ScriptableObject>("SO/LogItemData") as ItemData, 1 },
                { Resources.Load<ScriptableObject>("SO/StoneItemData") as ItemData, 1 }
            };
        TryCraftItem(Resources.Load<ScriptableObject>("SO/SwordItemData") as ItemData, requiredItemsForSword);
    }

    private void MakeCraftingTable() // 제작대 스크립터블 오브젝트 추가 필요
    {
        Dictionary<ItemData, int> requiredItemsForSword = new Dictionary<ItemData, int>
            {
                { Resources.Load<ScriptableObject>("SO/LogItemData") as ItemData, 4 }
            };
        TryCraftItem(Resources.Load<ScriptableObject>("SO/CraftingTableData") as ItemData, requiredItemsForSword);
    }
}
