using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private IEnumerator Start()
    {
        UILoadingScene loadingUI = null;
        // 0. 로딩씬 UI open
        Managers.Resource.LoadAsync<UnityEngine.Object>("UILoadingScene.prefab", (obj) =>
        {
            Managers.UI.ShowSceneUI<UILoadingScene>();
            Managers.UI.TryGetSceneUI(out loadingUI);
        });

        var waitWhile = new WaitWhile(() => loadingUI == null);
        yield return waitWhile;

        // 1. 리소스 로드
        ResourceLoad((key, count, total) =>
        {
            loadingUI.ReceiveCallbacks($"Resource Loading ... ({count} / {total})");
            if (count == total)
            {
                // 2. 맵 생성
                Managers.Game.GenerateWorldAsync((progress, argument) =>
                {
                    // 맵 생성 진행 중 콜백
                    loadingUI.ReceiveCallbacks($"{argument} ({progress * 100:00}%)");
                },
                () =>
                {
                    // 맵 생성 완료 시 콜백
                    // 3. 객체 생성, 초기화
                    loadingUI.ReceiveCallbacks($"Game Initialize ...");
                    SpawnPlayer();
                    UIInitialize();
                    Managers.Data.InitializeRecipeData();

                    Managers.Sound.Init();

                    Managers.Game.World.InitializeWorldNavMeshBuilder(callback: op => 
                    {
                        // 4. NavMesh 생성
                        var mon = Managers.Resource.GetCache<GameObject>("Skeleton.prefab");
                        Instantiate(mon);
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
}