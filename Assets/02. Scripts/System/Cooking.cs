

// 2024. 01. 16 Byun Jeongmin
using System.Collections.Generic;

public class Cooking : CraftBase
{
    public Player Owner { get; private set; }

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
    }

    public override bool CheckItems(List<RecipeSO.Ingredient> items)
    {
        return base.CheckItems(items);
    }

    public override void AddItems(List<RecipeSO.Ingredient> items)
    {
        base.AddItems(items);
    }
}
