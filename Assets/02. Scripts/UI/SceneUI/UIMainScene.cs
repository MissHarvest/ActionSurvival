
using UnityEngine;

public class UIMainScene : UIScene
{
    #region Enums

    enum GameObjects
    {
        QuickSlotController,
        PC,
        HpSlider,
        Menu,
    }

    #endregion

    public GameObject UIBoss => Get<GameObject>((int)GameObjects.HpSlider);
    public GameObject UIMenu => Get<GameObject>((int)GameObjects.Menu);

    #region Initialize

    private void Start()
    {
        Initialize();

        Get<GameObject>((int)GameObjects.PC).SetActive(true);
    }

    public override void Initialize()
    {
        base.Initialize();

        Bind<GameObject>(typeof(GameObjects));
        Get<GameObject>((int)GameObjects.PC).BindEvent((x) =>
        {
            Get<GameObject>((int)GameObjects.PC).SetActive(false);
        });
    }
    #endregion
}
