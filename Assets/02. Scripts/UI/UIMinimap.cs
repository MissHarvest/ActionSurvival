using UnityEngine;

// 2024. 02. 07 Byun Jeongmin
public class UIMinimap : UIPopup
{
    enum GameObjects
    {
        Map,
        Exit,
    }

    private Transform _map;

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(GameObjects));
        Get<GameObject>((int)GameObjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Initialize();
        _map = Get<GameObject>((int)GameObjects.Map).transform;
        gameObject.SetActive(false);
    }
}
