using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    public GameObject[] objectsBeLoaded;

    private void Awake()
    {
        Managers.Resource.LoadAllAsync<UnityEngine.Object>("MainScene", (key, count, total) =>
        {
            if (count == total)
            {
                var player = Managers.Resource.GetCache<GameObject>("Player.prefab");
                player = Instantiate(player);
                player.name = "Player";
                virtualCamera.Follow = GameManager.Instance.Player.ViewPoint;
                virtualCamera.LookAt = GameManager.Instance.Player.ViewPoint;

                Managers.Data.InitializeRecipeData();
                Managers.Sound.Init();

                UIInitialize();
            }
        });
    }

    private void UIInitialize()
    {
        Managers.UI.ShowSceneUI<UIMainScene>();
        Managers.UI.LoadPopupUIs();
    }
}
