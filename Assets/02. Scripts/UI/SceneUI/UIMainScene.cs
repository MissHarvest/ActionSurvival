
using UnityEngine;

public class UIMainScene : UIScene
{
    #region Enums

    enum GameObjects
    {
        QuickSlotController,
        PC,
        HpSlider
    }

    #endregion

    public GameObject UIBoss => Get<GameObject>((int)GameObjects.HpSlider);

    #region Initialize

    private void Start()
    {
        Initialize();

#if UNITY_ANDROID
        Get<GameObject>((int)GameObjects.PC).SetActive(false);
#endif
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
