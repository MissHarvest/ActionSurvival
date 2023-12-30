
using UnityEngine;

public class UIMainScene : UIScene
{
    #region Enums

    enum GameObjects
    {
        QuickSlotController,
    }

    #endregion



    #region Initialize

    private void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();

        Bind<GameObject>(typeof(GameObjects));
    }

    #endregion
}
