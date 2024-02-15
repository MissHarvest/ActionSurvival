using Cinemachine;
using System;
using System.Collections;
using System.Diagnostics;
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
                Stopwatch watch = new Stopwatch();
                watch.Start();
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

                    // Player 정보 로드
                    // SaveData.CheckPlayerData

                    loadingUI.ReceiveCallbacks($"Game Initialize ...");

                    Managers.Data.InitializeRecipeData();
                    Managers.Sound.Init();

                    Managers.Game.GenerateNavMeshAsync(callback: op =>
                    {
                        // 4. NavMesh 생성
                        SpawnPlayer();
                        UIInitialize();
                        Managers.Game.Init();

                        var camera = Managers.Resource.GetCache<GameObject>("MinimapCamera.prefab");
                        Instantiate(camera);

                        watch.Stop();
                        UnityEngine.Debug.Log($"Game Generated {watch.ElapsedMilliseconds} ms");
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
        player = Instantiate(player, new Vector3(-40f, 1f, 22f), Quaternion.identity);
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