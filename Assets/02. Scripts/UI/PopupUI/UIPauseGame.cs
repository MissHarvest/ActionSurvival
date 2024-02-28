using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPauseGame : UIPopup
{
    enum Buttons
    {
        SoundButton,
        GameHelperButton,
        ExitButton,
    }

    enum GameObjects
    {
        Block,
    }

    public override void Initialize()
    {
        base.Initialize();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Get<GameObject>((int)GameObjects.Block).BindEvent((x) =>
        {
            Managers.UI.ClosePopupUI(this);
        });
    }

    private void Awake()
    {
        Initialize();
        BindEventOfButtons();
        gameObject.SetActive(false);
    }

    private void BindEventOfButtons()
    {
        Get<Button>((int)Buttons.SoundButton).gameObject.BindEvent((x) =>
        {
            Managers.UI.ShowPopupUI<UISoundSetting>();
        });

        Get<Button>((int)Buttons.GameHelperButton).gameObject.BindEvent((x) =>
        {
            Managers.UI.ShowPopupUI<UITipInformation>();
        });

        Get<Button>((int)Buttons.ExitButton).gameObject.BindEvent((x) =>
        {
            Managers.UI.ShowPopupUI<UIWarning>().SetWarning(
                "저장 후 게임을 종료하시겠습니까?",
                UIWarning.Type.YesNo,
                () =>
                {
                    GameManager.Instance.SaveGameData();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
                });
        });
    }
}
