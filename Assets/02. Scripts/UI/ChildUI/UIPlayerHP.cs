using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerHP : MonoBehaviour
{
    public TextMeshProUGUI HPText { get; private set; }
    private float _playerMaxHP;

    private void Awake()
    {
        HPText = GetComponent<TextMeshProUGUI>();

        var playerConditions = GameObject.Find("Player").GetComponent<PlayerConditionHandler>();
        playerConditions.HP.OnUpdated += PrintHP;
        _playerMaxHP = playerConditions.HP.maxValue;
    }

    private void PrintHP(float percentage)
    {
        HPText.text = $"{(int)Mathf.Round(_playerMaxHP * percentage)}";
    }
}
