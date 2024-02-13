using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// 2024. 01. 27 Park Jun Uk
public class UITitleScene : UIScene
{
    enum Buttons
    {
        NewGameButton,
        ContinueButton,
        SettingButton,
    }

    enum GameObjects
    {
        ButtonGroup,
    }

    public override void Initialize()
    {
        base.Initialize();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        Get<Button>((int)Buttons.NewGameButton).onClick.AddListener(() =>
        {
            if(SaveGame.ExistFiles())
            {
                var ui = Managers.UI.ShowPopupUI<UIWarning>();
                ui.SetWarning("세이브 파일이 있습니다.\n 정말 새로 시작하나요?", StartNewGame);
            }
            else
            {
                StartNewGame();
            }
        });

        Get<Button>((int)Buttons.NewGameButton).gameObject.BindEvent((x) =>
        {
            Managers.Sound.PlayEffectSound(transform.position,"ButtonHover");
        }, UIEvents.PointerEnter);

        Get<Button>((int)Buttons.ContinueButton).onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Main Scene");
        });

        Get<Button>((int)Buttons.ContinueButton).gameObject.BindEvent((x) =>
        {
            Managers.Sound.PlayEffectSound(transform.position, "ButtonHover");
        }, UIEvents.PointerEnter);


        Get<Button>((int)Buttons.SettingButton).onClick.AddListener(() =>
        {
            Managers.UI.ShowPopupUI<UISoundSetting>();
        });

        Get<Button>((int)Buttons.SettingButton).gameObject.BindEvent((x) =>
        {
            Managers.Sound.PlayEffectSound(transform.position, "ButtonHover");
        }, UIEvents.PointerEnter);


        Get<GameObject>((int)GameObjects.ButtonGroup).SetActive(false);
    }
    private void Awake()
    {
        Initialize();        
    }

    public void ActivateButtons()
    {
        Get<GameObject>((int)GameObjects.ButtonGroup).SetActive(true);
    }

    private void StartNewGame()
    {
        SaveGame.DeleteAllFiles();
        SceneManager.LoadScene("Main Scene");
    }
}
