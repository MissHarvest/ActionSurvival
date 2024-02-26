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
        YesOnly,
        YesNo,
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

    private bool _show = false;

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
        //Load();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Get<Button>((int)Buttons.YesButton).onClick.RemoveAllListeners();        
    }

    IEnumerator HideAfterSec(float sec)
    {
        yield return new WaitForSeconds(sec);
        Managers.UI.ClosePopupUI(this);
    }

    private void PrintWarning(string warning)
    {
        Get<TextMeshProUGUI>((int)Texts.WarningText).text = warning;
        Managers.Sound.PlayEffectSound("Warning");
    }

    private void SetButtonsActive(Type type)
    {
        Get<Button>((int)Buttons.YesButton).gameObject.SetActive(true);
        Get<Button>((int)Buttons.NoButton).gameObject.SetActive(type == Type.YesNo);
    }

    public void SetWarning(string warning)
    {
        if (_show) return;
        _show = true;
        PrintWarning(warning);
        StartCoroutine(HideAfterSec(1.0f));
    }

    public void SetWarning(string warning, UnityAction yesAction)
    {
        SetWarning(warning, Type.YesOnly, yesAction);
    }

    public void SetWarning(string warning, Type type, UnityAction yesAction)
    {
        PrintWarning(warning);
        SetButtonsActive(type);
        Get<Button>((int)Buttons.YesButton).onClick.AddListener(() =>
        {
            Close();
            yesAction.Invoke();
        });
    }

    public void SetWarning(string warning, Type type, UnityAction yesAction, bool once)
    {
        if(once)
        {
            //if(!_onceWarnings.Contains(warning))
            //{
            //    AddWarning(warning);
            //    SetWarning(warning, type, yesAction);
            //    StartCoroutine(Save());
            //}
            //else
            //{
            //    Close();
            //}
        }
        else
        {
            //SetWarning(warning, yesAction);
        }
    }

    private void OnDisable()
    {
        Get<Button>((int)Buttons.YesButton).gameObject.SetActive(false);
        Get<Button>((int)Buttons.NoButton).gameObject.SetActive(false);
    }

    private void Close()
    {
        _show = false;
        Managers.UI.ClosePopupUI(this);
    }
}
