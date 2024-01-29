using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerHP : UIPlayerCondition
{
    protected override void BindToPlayerCondition()
    {
        var hp = GameObject.Find("Player").GetComponent<PlayerConditionHandler>().HP;
        _maxConditionValue = hp.maxValue;
        hp.OnUpdated += OnConditionUpdated;
    }
}
