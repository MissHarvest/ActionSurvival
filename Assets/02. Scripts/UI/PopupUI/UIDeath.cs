using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIDeath : UIPopup
{
    enum Buttons
    {
        MainButton,
    }

    public override void Initialize()
    {
        base.Initialize();
        Bind<Button>(typeof(Buttons));
        Get<Button>((int)Buttons.MainButton).onClick.AddListener(LoadTitleScene);
    }

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
    }

    private void LoadTitleScene()
    {
        Managers.UI.ClosePopupUI(this);
        SceneManager.LoadScene("TitleScene");
        CoroutineManagement.Instance.StopAllCoroutines();
    }
}
