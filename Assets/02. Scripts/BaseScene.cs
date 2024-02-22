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
                GameManager.Instance.Init();

                var player = Managers.Resource.GetCache<GameObject>("Player.prefab");
                player = Instantiate(player);
                player.name = "Player";
                virtualCamera.Follow = GameManager.Instance.Player.ViewPoint;
                virtualCamera.LookAt = GameManager.Instance.Player.ViewPoint;

                Managers.Sound.Init();
                Managers.UI.ShowSceneUI<UIMainScene>();
                Managers.UI.LoadPopupUIs();
                Managers.Data.InitializeRecipeData();

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
