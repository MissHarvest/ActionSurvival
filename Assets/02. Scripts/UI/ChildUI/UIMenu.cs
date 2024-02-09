using System.Collections;
using System.Collections.Generic;
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
    }

    private bool _isHidden = false;
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
        if (_isHidden)
        {
            ShowMenu();
        }
        else
        {
            HideMenu();
        }        
    }

    private void ShowMenu()
    {
        _rectTransform.anchoredPosition += new Vector2(-_offset, 0);
        _isHidden = false;
    }

    private void HideMenu()
    {
        _rectTransform.anchoredPosition += new Vector2(_offset, 0);
        _isHidden = true;
    }
}
