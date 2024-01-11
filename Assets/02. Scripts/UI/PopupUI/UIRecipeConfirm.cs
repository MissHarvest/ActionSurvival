using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 2024. 01. 11 Byun Jeongmin
public class UIRecipeConfirm : UIPopup
{
    enum Gameobjects
    {
        Character,
        Exit,
        YesButton,
        NoButton
    }

    private Transform _character;

    private GameObject _yesButton;
    private GameObject _noButton;

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });

        _yesButton = Get<GameObject>((int)Gameobjects.YesButton);
        _noButton = Get<GameObject>((int)Gameobjects.NoButton);

        _yesButton.BindEvent((x) => { OnCraftConfirmed(); });
        _noButton.BindEvent((x) => { OnCraftCanceled(); });
    }

    private void Awake()
    {
        Initialize();
        _character = Get<GameObject>((int)Gameobjects.Character).transform;
        gameObject.SetActive(false);
    }

    private void OnCraftConfirmed()
    {
        Debug.Log("제작 성공");

        // 예 버튼이 눌렸을 때
        gameObject.SetActive(false);
    }

    private void OnCraftCanceled()
    {
        Debug.Log("제작 취소");

        // 아니오 버튼이 눌렸을 때
        gameObject.SetActive(false);
    }
}
