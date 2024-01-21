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
                // 2. 맵 생성
                Managers.Game.GenerateWorldAsync((progress, argument) =>
                {
                    // 맵 생성 진행 중 콜백
                    Debug.Log(progress + ": " + argument);
                },
                () =>
                {
                    // 맵 생성 완료 시 콜백
                    // 3. 객체 생성, 초기화
                    SpawnPlayer();
                    UIInitialize();
                    Managers.Game.World.InitializeWorldNavMeshBuilder(callback: op => 
                    {
                        // 4. NavMesh 생성
                        var mon = Managers.Resource.GetCache<GameObject>("Skeleton.prefab");
                        Instantiate(mon);
                        SpawnObject();
                        Managers.Game.Init();
                    });
                });
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
        player = Instantiate(player, Vector3.up, Quaternion.identity);
        player.name = "Player";
        virtualCamera.Follow = Managers.Game.Player.ViewPoint;
        virtualCamera.LookAt = Managers.Game.Player.ViewPoint;
    }

    private void UIInitialize()
    {
        Managers.UI.ShowSceneUI<UIMainScene>();
        Managers.UI.LoadPopupUIs();
    }

    private void SpawnObject()
    {
        var obj = Managers.Resource.GetCache<GameObject>("Mushroom.prefab");
        Instantiate(obj);
        obj = Managers.Resource.GetCache<GameObject>("Rock.prefab");
        Instantiate(obj);
        obj = Managers.Resource.GetCache<GameObject>("TreeA.prefab");
        Instantiate(obj);
        obj = Managers.Resource.GetCache<GameObject>("TreeAStump.prefab");
        Instantiate(obj);
    }
}