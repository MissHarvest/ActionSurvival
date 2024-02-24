using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITip : UIPopup
{
    enum Texts
    {
        TipText,
    }

    enum Buttons
    {
        YesButton,
        NoButton,
    }

    public override void Initialize()
    {
        base.Initialize();
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Get<Button>((int)Buttons.NoButton).onClick.AddListener(Close);
    }

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
    }

    public void Set(TipData tipData, Action action)
    {
        PrintTip(tipData.summary);

        Get<Button>((int)Buttons.YesButton).onClick.RemoveAllListeners();

        Get<Button>((int)Buttons.YesButton).onClick.AddListener(
            () =>
            {
                Close();
                action?.Invoke();
            });
        
        Time.timeScale = 0.0f;
    }

    private void PrintTip(string tip)
    {
        Get<TextMeshProUGUI>((int)Texts.TipText).text = tip;
        Managers.Sound.PlayEffectSound(transform.position, "Tip");
    }

    private void Close()
    {
        Managers.UI.ClosePopupUI(this);
        Time.timeScale = 1.0f;
    }
}
