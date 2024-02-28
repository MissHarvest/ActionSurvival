// 2024. 01. 16 Byun Jeongmin
using System.Collections.Generic;

public class Cooking : CraftBase
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
