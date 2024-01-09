using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIOptionButton : UIBase
{
    enum Texts
    {
        Behavior,
    }

    public override void Initialize()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

    }

    private void Awake()
    {
        Initialize();
    }

    public void SetText(string text)
    {
        Get<TextMeshProUGUI>((int)Texts.Behavior).text = text;
    }

    public void Bind(UnityAction call)
    {
        GetComponent<Button>().onClick.AddListener(call);
    }
}
