using System.Collections.Generic;

// 2024. 01. 11 Byun Jeongmin
public class Recipe : CraftBase
{
    public Player Owner { get; private set; }
    private UIRecipe _recipeUI;

    public override void Awake()
    {
        base.Awake();
        Owner = Managers.Game.Player;
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
