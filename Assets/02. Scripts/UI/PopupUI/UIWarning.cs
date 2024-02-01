using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class UIWarning : UIPopup
{
    enum Texts
    {
        WarningText,
    }

    public override void Initialize()
    {
        base.Initialize();
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Managers.Sound.PlayEffectSound(transform.position, "Warning");
        StartCoroutine(HideAfterSec(1.0f));
    }

    IEnumerator HideAfterSec(float sec)
    {
        yield return new WaitForSeconds(sec);
        Managers.UI.ClosePopupUI(this);
    }

    public void SetWarning(string warning)
    {
        Get<TextMeshProUGUI>((int)Texts.WarningText).text = warning;
    }
}
