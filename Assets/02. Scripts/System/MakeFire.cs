using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Lee gyuseong 24.02.13

public class MakeFire : MonoBehaviour
{
    //Player.System에 달아줄 컴포넌트 형식
    //모닥불, 화로에 Interact 시 UIMakeFire를 ShowPopup 시킨다.

    private UIMakeFire _makeFireUI;

    public void OnMakeFireShowAndHide()
    {
        if (_makeFireUI == null)
        {
            _makeFireUI = Managers.UI.ShowPopupUI<UIMakeFire>();
            return;
        }

        if (_makeFireUI.gameObject.activeSelf)
        {
            Managers.UI.ClosePopupUI(_makeFireUI);
        }
        else
        {
            Managers.UI.ShowPopupUI<UIMakeFire>();
        }
    }
}
