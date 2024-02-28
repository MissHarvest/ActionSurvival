using System.Collections.Generic;

// 2024. 01. 11 Byun Jeongmin
public class Recipe : CraftBase
{
    public override bool CheckItems(List<RecipeSO.Ingredient> items)
    {
        return base.CheckItems(items);
    }

    public override void ConsumeItems(List<RecipeSO.Ingredient> items)
    {
        base.ConsumeItems(items);
    }

    public override void AddItems(List<RecipeSO.Ingredient> items)
    {
        base.AddItems(items);
    }
}
