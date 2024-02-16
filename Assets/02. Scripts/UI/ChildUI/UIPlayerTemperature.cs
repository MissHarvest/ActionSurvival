
// 2024-02-15 WJY
using TMPro;
using UnityEngine;

public class UIPlayerTemperature : UIPlayerCondition
{
    enum Texts
    {
        ConditionText,
    }

    public override void Initialize()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
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
}