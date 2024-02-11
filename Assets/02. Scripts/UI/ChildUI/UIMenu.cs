using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
        arrowUI.ActivateArrow(go.transform.position, new Vector2(0, rect.sizeDelta.y));
        go.BindEvent((x) =>
        {
            // arrow UI 가 닫히는 것과 CraftButton 으로 UI 가 열리는 것의 순서가 어떻게 정해질까
            Managers.UI.ClosePopupUI(arrowUI);

            var evtHandler = Utility.GetOrAddComponent<UIEventHandler>(go);
            evtHandler.OnPointerDownEvent = null;
        }, UIEvents.PointerDown);
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
