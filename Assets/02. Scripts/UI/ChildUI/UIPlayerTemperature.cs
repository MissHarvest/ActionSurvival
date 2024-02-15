
// 2024-02-15 WJY
using UnityEngine;

public class UIPlayerTemperature : UIPlayerCondition
{
    protected override void BindToPlayerCondition()
    {
        var temperature = GameObject.Find("Player").GetComponent<PlayerConditionHandler>().Temperature;
        _maxConditionValue = temperature.maxValue;
        temperature.OnUpdated += OnConditionUpdated;
        OnConditionUpdated(temperature.GetPercentage());
    }
}