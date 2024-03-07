
// 2024-02-15 WJY
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerTemperature : UIPlayerCondition
{
    enum Images
    {
        Icon,
    }

    enum Texts
    {
        ConditionText,
    }

    public Sprite[] sprites;

    public override void Initialize()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));

        GameManager.Season.OnSeasonChanged += OnSeasonChanged;
    }

    protected override void OnConditionUpdated(float percentage)
    {
        Get<TextMeshProUGUI>((int)Texts.ConditionText).text = $"{(int)Mathf.Round(_maxConditionValue * percentage)}";
    }

    protected override void BindToPlayerCondition()
    {
        var temperature = GameObject.Find("Player").GetComponent<PlayerConditionHandler>().Temperature;
        _maxConditionValue = temperature.maxValue;
        temperature.OnUpdated += OnConditionUpdated;
        OnConditionUpdated(temperature.GetPercentage());
    }

    private void OnSeasonChanged(Season.State state)
    {
        Get<Image>((int)Images.Icon).sprite = sprites[(int)state];
    }
}