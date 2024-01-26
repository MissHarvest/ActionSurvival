using System.Collections.Generic;

// 2024. 01. 11 Byun Jeongmin
public class UIRecipe : UICraftBase
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        
        string currentToolName = Managers.Game.Player.ToolSystem.GetToolName(Managers.Game.Player.ToolSystem.ItemInUse);

        // 고급 레시피 UI 활성화
        if (currentToolName == "Handable_CraftingTable")
        {
            SetAdvancedRecipeUIActive(true);
        }
        else
        {
            SetAdvancedRecipeUIActive(false);
        }
    }

    // 고급 레시피 UI의 활성화 여부를 설정하는 메서드
    public void SetAdvancedRecipeUIActive(bool active)
    {
        foreach (var slot in _uiRecipeSlots)
        {
            bool isAdvancedRecipe = Managers.Data.recipeDataList[slot.Index].IsAdvancedRecipe;
            slot.gameObject.SetActive(active || (!isAdvancedRecipe && !active));
        }
    }

    protected override UIConfirmBase GetConfirmPopup()
    {
        return Managers.UI.ShowPopupUI<UIRecipeConfirm>();
    }

    protected override List<RecipeSO> GetDataList()
    {
        return Managers.Data.recipeDataList;
    }
}
