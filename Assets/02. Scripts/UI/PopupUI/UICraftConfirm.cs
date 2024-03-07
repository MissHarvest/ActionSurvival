using System.Collections;
using TMPro;
using UnityEngine;

// 2024. 02. 02 Byun Jeongmin
public class UICraftConfirm : UIPopup
{
    enum Texts
    {
        CraftText,
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
        //Managers.Sound.PlayEffectSound(transform.position, "Warning");
        StartCoroutine(HideAfterSec(1f));
    }

    IEnumerator HideAfterSec(float sec)
    {
        yield return new WaitForSeconds(sec);
        Managers.UI.ClosePopupUI(this);
    }

    public void SetCraft(string craft)
    {
        Get<TextMeshProUGUI>((int)Texts.CraftText).text = craft;
    }
}
