using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 2024. 01. 16 Byun Jeongmin
public class Cooking : MonoBehaviour
{
    public Player Owner { get; private set; }
    private UICooking _cookingUI;


    private void Awake()
    {
        Debug.Log("Cooking Awake");
        Owner = Managers.Game.Player;
        var input = Owner.Input;
        input.InputActions.Player.Interact.started += OnCookingShowAndHide; //E키로 요리 UI 띄움
    }

    private void Start()
    {
        Debug.Log("Cooking Start");
    }

    public void OnCookingShowAndHide(InputAction.CallbackContext context)
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
