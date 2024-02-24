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

    private void OnEnable()
    {
        Time.timeScale = 0.0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1.0f;
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
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }
}
