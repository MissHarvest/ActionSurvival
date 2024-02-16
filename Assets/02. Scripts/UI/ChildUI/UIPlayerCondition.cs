using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIPlayerCondition : UIBase
{
    enum Images
    {
        Icon,
    }

    enum Sliders
    {
        ConditionSlider,
    }

    enum Texts
    {
        ConditionText,
    }

    protected float _maxConditionValue;

    public override void Initialize()
    {
        Bind<Image>(typeof(Images));
        Bind<Slider>(typeof(Sliders));
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        BindToPlayerCondition();
    }

    protected abstract void BindToPlayerCondition();

    protected virtual void OnConditionUpdated(float percentage)
    {
        Get<Slider>((int)Sliders.ConditionSlider).value = percentage;
        Get<TextMeshProUGUI>((int)Texts.ConditionText).text = $"{(int)Mathf.Round(_maxConditionValue * percentage)}";
    }
}
