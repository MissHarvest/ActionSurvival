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
        Debug.Log("���� ����");

        // �� ��ư�� ������ ��
        gameObject.SetActive(false);
    }

    private void OnCraftCanceled()
    {
        Debug.Log("���� ���");

        // �ƴϿ� ��ư�� ������ ��
        gameObject.SetActive(false);
    }
}
