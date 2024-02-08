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

    private bool _isHidden = false;
    public override void Initialize()
    {
        Bind<Button>(typeof(Buttons));
    }

    private void Awake()
    {
        Initialize();
        Get<Button>((int)Buttons.Toggle).onClick.AddListener(ToggleMenu);
    }

    private void ToggleMenu()
    {
        if(_isHidden)
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
        transform.position += new Vector3(-340, 0, 0);
        _isHidden = false;
    }

    private void HideMenu()
    {
        transform.position += new Vector3(340, 0, 0);
        _isHidden = true;
    }
}
