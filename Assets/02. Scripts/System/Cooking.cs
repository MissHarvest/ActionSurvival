

// 2024. 01. 16 Byun Jeongmin
public class Cooking : CraftBase
{
    public Player Owner { get; private set; }
    private UICooking _cookingUI;

    public override void Awake()
    {
        base.Awake();
        Owner = Managers.Game.Player;
        var input = Owner.Input;
    }

    public override void Start()
    {
        base.Start();
    }

    public void OnCookingShowAndHide()
    {
        if (_cookingUI == null)
        {
            _cookingUI = Managers.UI.ShowPopupUI<UICooking>();
            return;
        }

        if (_cookingUI.gameObject.activeSelf)
        {
            Managers.UI.ClosePopupUI(_cookingUI);
        }
        else
        {
            Managers.UI.ShowPopupUI<UICooking>();
        }
    }
}
