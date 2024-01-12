using Cinemachine;
using System;
using UnityEngine;

public class WJY_VoxelTileTestScene : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private void Awake()
    {
        // 1. 리소스 로드
        ResourceLoad((key, count, total) =>
        {
            if (count == total)
            {
                // 2. 객체 생성, 초기화
                SpawnPlayer();
                UIInitialize();
                GenerateMap();
            }
        });
    }

    private void ResourceLoad(Action<string, int, int> callback)
    {
        Managers.Resource.LoadAllAsync<UnityEngine.Object>("MainScene", callback);
    }

    private void SpawnPlayer()
    {
        var player = Managers.Resource.GetCache<GameObject>("Player.prefab");
        player = Instantiate(player);
        player.name = "Player";
        virtualCamera.Follow = Managers.Game.Player.ViewPoint;
        virtualCamera.LookAt = Managers.Game.Player.ViewPoint;
    }

    private void UIInitialize()
    {
        Managers.UI.ShowSceneUI<UIMainScene>();
        Managers.UI.LoadPopupUIs();
    }

    private void GenerateMap()
    {
        var obj = Managers.Resource.GetCache<GameObject>("TestGround.prefab");
        Instantiate(obj);
        obj = Managers.Resource.GetCache<GameObject>("Mushroom.prefab");
        Instantiate(obj);
        obj = Managers.Resource.GetCache<GameObject>("Rock.prefab");
        Instantiate(obj);
        obj = Managers.Resource.GetCache<GameObject>("TreeA.prefab");
        Instantiate(obj);
        obj = Managers.Resource.GetCache<GameObject>("TreeAStump.prefab");
        Instantiate(obj);
    }
}