using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerHunger : MonoBehaviour
{
    private TextMeshProUGUI hungerText;
    private float _playerMaxHunger;

    private void Awake()
    {
        hungerText = GetComponent<TextMeshProUGUI>();

        var playerConditions = GameObject.Find("Player").GetComponent<PlayerConditionHandler>();
        playerConditions.Hunger.OnUpdated += PrintHunger;
        _playerMaxHunger = playerConditions.Hunger.maxValue;
    }

    private void PrintHunger(float percentage)
    {
        hungerText.text = $"{(int)Mathf.Round(_playerMaxHunger * percentage)}";
    }
}
