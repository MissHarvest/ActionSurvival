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
                //Managers.Game.Init();

                var player = Managers.Resource.GetCache<GameObject>("Player.prefab");
                player = Instantiate(player);
                player.name = "Player";
                virtualCamera.Follow = Managers.Game.Player.ViewPoint;
                virtualCamera.LookAt = Managers.Game.Player.ViewPoint;

            }
        });
    }
}
