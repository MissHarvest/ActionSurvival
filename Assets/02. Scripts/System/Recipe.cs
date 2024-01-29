using UnityEngine.InputSystem;

// 2024. 01. 11 Byun Jeongmin
public class Recipe : CraftBase
{
    public Player Owner { get; private set; }
    private InventorySystem _inventory;
    private UIRecipe _recipeUI;

    public override void Awake()
    {
        base.Awake();
        Owner = Managers.Game.Player;
        var input = Owner.Input;
        input.InputActions.Player.Recipe.started += OnRecipeShowAndHide;
    }

    public override void Start()
    {
        base.Start();
        _inventory = Managers.Game.Player.Inventory;
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
        }
    }
}
