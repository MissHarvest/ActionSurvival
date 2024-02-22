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
        SetAdvancedRecipeUIActive(0);
    }

    protected override void GetData()
    {
        _recipeOrCookingList = Managers.Data.recipeDataList;
        _craftBase = GameManager.Instance.Player.Recipe;
    }
}
