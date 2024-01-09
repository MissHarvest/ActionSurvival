using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerHP : MonoBehaviour
{
    private TextMeshProUGUI HPText { get; }
    private float _playerMaxHP;

    public UIPlayerHP()
    {
        HPText = GetComponent<TextMeshProUGUI>();
    }

    private void Awake()
    {
        var playerConditions = GameObject.Find("Player").GetComponent<PlayerConditionHandler>();
        playerConditions.HP.OnUpdated += PrintHP;
        _playerMaxHP = playerConditions.HP.maxValue;
    }

    private void PrintHP(float percentage)
    {
        HPText.text = $"{(int)Mathf.Round(_playerMaxHP * percentage)}";
    }
}
