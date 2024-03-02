using UnityEngine;
using UnityEngine.UI;

public class UIMenu : UIBase
{
    enum Buttons
    {
        Toggle,
    }

    enum GameObjects
    {
        Container,
        InventoryButton,
        CraftButton,
    }

    public bool Hide { get; private set; } = false;
    private float _offset = 0.0f;
    private RectTransform _rectTransform;

    public override void Initialize()
    {
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
    }

    private void Awake()
    {
        Initialize();
        _rectTransform = GetComponent<RectTransform>();

        Get<Button>((int)Buttons.Toggle).onClick.AddListener(ToggleMenu);
        _offset = Get<GameObject>((int)GameObjects.Container).GetComponent<RectTransform>().sizeDelta.x;
    }

    private void ToggleMenu()
    {
        if (Hide)
        {
            ShowMenu();
        }
        else
        {
            HideMenu();
        }        
    }

    public void HighLightCraftButton()
    {
        ShowMenu();
        HighLight(Get<GameObject>((int)GameObjects.CraftButton));
    }

    public void HighLightInventoryButton()
    {
        ShowMenu();
        HighLight(Get<GameObject>((int)GameObjects.InventoryButton));
    }

    private void HighLight(GameObject go)
    {
        var rect = go.GetComponent<RectTransform>();

        var arrowUI = Managers.UI.ShowPopupUI<UITutorialArrow>();
        if (arrowUI == null) return;
        arrowUI.ActivateArrow(go.transform.position, new Vector2(0, rect.sizeDelta.y));
    }

    public void ShowMenu()
    {
        if(Hide)
        {
            _rectTransform.anchoredPosition += new Vector2(-_offset, 0);
            Hide = false;
        }
    }

    public void HideMenu()
    {
        if(!Hide)
        {
            _rectTransform.anchoredPosition += new Vector2(_offset, 0);
            Hide = true;
        }
    }
}
