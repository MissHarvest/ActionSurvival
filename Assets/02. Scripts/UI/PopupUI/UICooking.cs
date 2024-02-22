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
        _craftBase = Managers.Game.Player.Cooking;
    }
}