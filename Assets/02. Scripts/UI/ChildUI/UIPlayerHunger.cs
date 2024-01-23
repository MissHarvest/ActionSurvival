using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerHunger : UIPlayerCondition
{
    protected override void BindToPlayerCondition()
    {
        var hunger = GameObject.Find("Player").GetComponent<PlayerConditionHandler>().Hunger;
        _maxConditionValue = hunger.maxValue;
        hunger.OnUpdated += OnConditionUpdated;
    }
}
