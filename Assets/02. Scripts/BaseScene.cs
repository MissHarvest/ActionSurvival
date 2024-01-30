using Cinemachine;
using UnityEngine;

public class BaseScene : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    public GameObject[] objectsBeLoaded;

    private void Awake()
    {
        Managers.Resource.LoadAllAsync<UnityEngine.Object>("MainScene", (key, count, total) => 
        {
            if (count == total)
            {
                Managers.Game.Init();

                var player = Managers.Resource.GetCache<GameObject>("Player.prefab");
                player = Instantiate(player);
                player.name = "Player";
                virtualCamera.Follow = Managers.Game.Player.ViewPoint;
                virtualCamera.LookAt = Managers.Game.Player.ViewPoint;

                Managers.Sound.Init();
                Managers.UI.ShowSceneUI<UIMainScene>();
                Managers.UI.LoadPopupUIs();
                Managers.Data.InitializeRecipeData();
                Managers.Game.Player.Tutorial.Initialize();

                if (objectsBeLoaded.Length > 0)
                {
                    foreach(var obj in objectsBeLoaded)
                    {
                        obj.SetActive(true);
                    }
                }

                //var mon = Managers.Resource.GetCache<GameObject>("FireRabbitMon.prefab");
                //Instantiate(mon);
            }
        });
    }
}
