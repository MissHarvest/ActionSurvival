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
        LoadingText,
    }

    public override void Initialize()
    {
        base.Initialize();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        Get<Button>((int)Buttons.NewGameButton).onClick.AddListener(() =>
        {
            CheckFirebaseDataAndStartNewGame();
        });

        Get<Button>((int)Buttons.NewGameButton).gameObject.BindEvent((x) =>
        {
            Managers.Sound.PlayEffectSound("ButtonHover");
        }, UIEvents.PointerEnter);

        Get<Button>((int)Buttons.ContinueButton).onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Main Scene");
        });

        Get<Button>((int)Buttons.ContinueButton).gameObject.BindEvent((x) =>
        {
            Managers.Sound.PlayEffectSound("ButtonHover");
        }, UIEvents.PointerEnter);


        Get<Button>((int)Buttons.SettingButton).onClick.AddListener(() =>
        {
            Managers.UI.ShowPopupUI<UISoundSetting>();
        });

        Get<Button>((int)Buttons.SettingButton).gameObject.BindEvent((x) =>
        {
            Managers.Sound.PlayEffectSound("ButtonHover");
        }, UIEvents.PointerEnter);


        Get<GameObject>((int)GameObjects.ButtonGroup).SetActive(false);
        Get<GameObject>((int)GameObjects.LoadingText).SetActive(false);
    }
    private void Awake()
    {
        Initialize();        
    }

    private void Start()
    {
        Get<GameObject>((int)GameObjects.LoadingText).SetActive(true);
    }

    public void ActivateButtons()
    {
        Get<GameObject>((int)GameObjects.LoadingText).SetActive(false);
        Get<GameObject>((int)GameObjects.ButtonGroup).SetActive(true);
    }

    private void StartNewGame()
    {
        SaveGame.DeleteAllFiles();
        SaveGameUsingFirebase.DeleteAllFirebaseData();
        SceneManager.LoadScene("Main Scene");
    }

    private async void CheckFirebaseDataAndStartNewGame()
    {
        bool firebaseDataExists = await SaveGameUsingFirebase.CheckIfDataExists();
        bool localDataExists = SaveGame.ExistFiles();

        if (firebaseDataExists && localDataExists)
        {
            var ui = Managers.UI.ShowPopupUI<UIWarning>(pause:true);
            ui.SetWarning("세이브 파일이 있습니다.\n 정말 새로 시작하나요?",
                UIWarning.Type.YesNo,
                StartNewGame);
        }
        else
        {
            StartNewGame();
        }
    }
}
