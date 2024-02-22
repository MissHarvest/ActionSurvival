using UnityEngine;
using UnityEngine.InputSystem;

// 2024. 02. 07 Byun Jeongmin
public class UIMinimapButton : MonoBehaviour
{
    public Player Owner { get; private set; }
    private UIMinimap _minimapUI;

    private void Awake()
    {
        Owner = GameManager.Instance.Player;
        var input = Owner.Input;
        input.InputActions.Player.Minimap.started += OnMinimapShowAndHide;
    }

    private void OnMinimapShowAndHide(InputAction.CallbackContext context)
    {
        if (_minimapUI == null)
        {
            _minimapUI = Managers.UI.ShowPopupUI<UIMinimap>();
            return;
        }

        if (_minimapUI.gameObject.activeSelf)
        {
            Managers.UI.ClosePopupUI(_minimapUI);
        }
        else
        {
            Managers.UI.ShowPopupUI<UIMinimap>();
        }
    }
}
