using Cinemachine;
using UnityEngine;

public class BaseScene : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        Managers.Resource.LoadAllAsync<UnityEngine.Object>("MainScene", (key, count, total) => 
        {
            //Debug.Log($"{count}/{total}: {key} load success");

            if (count == total)
            {
                var player = Managers.Resource.GetCache<GameObject>("Player.prefab");
                player = Instantiate(player);
                player.name = "Player";
                virtualCamera.Follow = Managers.Game.Player.ViewPoint;
                virtualCamera.LookAt = Managers.Game.Player.ViewPoint;

                Managers.UI.ShowSceneUI<UIMainScene>();
                Managers.UI.LoadPopupUIs();
            }
        });

        //var player = Managers.Resource.Instantiate("Player");
        //player.name = "Player";
        //virtualCamera.Follow = Managers.Game.Player.ViewPoint;
        //virtualCamera.LookAt = Managers.Game.Player.ViewPoint;

        //// Managers.UI.ShowScene
        //Managers.UI.ShowSceneUI<UIMainScene>();
        //Managers.UI.LoadPopupUIs();
    }
}
