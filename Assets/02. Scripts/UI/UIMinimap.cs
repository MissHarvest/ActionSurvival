using UnityEngine;

// 2024. 02. 07 Byun Jeongmin
public class UIMinimap : UIPopup
{
    enum GameObjects
    {
        Exit,
    }

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(GameObjects));
        Get<GameObject>((int)GameObjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
    }
}
