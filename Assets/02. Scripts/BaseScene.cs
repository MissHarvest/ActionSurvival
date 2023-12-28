using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        var player = Managers.Resource.Instantiate("Player");
        player.name = "Player";
        virtualCamera.Follow = Managers.Game.Player.ViewPoint;
        virtualCamera.LookAt = Managers.Game.Player.ViewPoint;

        // Managers.UI.ShowScene
        Managers.UI.LoadPopupUIs();
    }
}
