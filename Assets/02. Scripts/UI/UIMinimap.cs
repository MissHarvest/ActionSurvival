using UnityEngine;

// 2024. 02. 07 Byun Jeongmin
public class UIMinimap : UIPopup
{
    enum GameObjects
    {
        Block,
    }

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(GameObjects));
        Get<GameObject>((int)GameObjects.Block).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
    }
}
