using UnityEngine.InputSystem;

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
}
