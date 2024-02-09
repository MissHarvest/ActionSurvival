using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIWarning : UIPopup
{
    public enum Type
    {
        Disappear,
        Confirm,
    }

    enum Texts
    {
        WarningText,
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
        SetButtonsActive(false);
    }

    private void OnEnable()
    {
        Get<Button>((int)Buttons.YesButton).onClick = null;
        Managers.Sound.PlayEffectSound(transform.position, "Warning");
    }

    IEnumerator HideAfterSec(float sec)
    {
        yield return new WaitForSeconds(sec);
        Managers.UI.ClosePopupUI(this);
    }

    private void PrintWarning(string warning)
    {
        Get<TextMeshProUGUI>((int)Texts.WarningText).text = warning;
    }

    private void SetButtonsActive(bool active)
    {
        Get<Button>((int)Buttons.YesButton).gameObject.SetActive(active);
        Get<Button>((int)Buttons.NoButton).gameObject.SetActive(active);
    }

    public void SetWarning(string warning, float time = 1.0f)
    {
        PrintWarning(warning);
        StartCoroutine(HideAfterSec(time));
    }

    public void SetWarning(string warning, UnityAction yesAction)
    {
        PrintWarning(warning);
        SetButtonsActive(true);
        Get<Button>((int)Buttons.YesButton).onClick.AddListener(() =>
        {
            Close();
            yesAction.Invoke();
        });
    }

    private void Close()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
