using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 11 Byun Jeongmin
public class UIRecipe : UIPopup
{
    enum Gameobjects
    {
        Contents,
        Exit,
    }

    private Transform _contents;

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Initialize();
        _contents = Get<GameObject>((int)Gameobjects.Contents).transform;

        gameObject.SetActive(false);
    }
}
