using System.Collections.Generic;

// 2024. 01. 16 Byun Jeongmin
public class UICooking : UICraftBase
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SetAdvancedRecipeUIActive(0);
    }

    protected override void GetData()
    {
        _recipeOrCookingList = Managers.Data.cookingDataList;
        _craftBase = GameManager.Instance.Player.Cooking;
    }

    protected override string GetConfirmationText(string displayName, int itemQuantity, int craftCount)
    {
        return $"{displayName}을(를) {itemQuantity} X {craftCount}개\n요리하시겠습니까?";
    }
}