using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerConditionHandler : MonoBehaviour
{
    public Player Player { get; private set; }
    [field: SerializeField] public Condition HP { get; private set; }
    [field: SerializeField] public Condition Hunger { get; private set; }
    [field: SerializeField] public Condition Temperature { get; private set; }

    private float _full = 0.7f;
    private float _hpRegenOfFullState = 0.2f;

    private void Awake()
    {
        Player = GetComponent<Player>();

        HP = new Condition(200);
        HP.regenRate = _hpRegenOfFullState;
        HP.OnBelowedToZero += Player.Die;

        Hunger = new Condition(100);
        Hunger.decayRate = 0.2f;
        Hunger.OnRecovered += OnHungerRecevered;
        Hunger.OnDecreased += OnHungerDecrease;
        Hunger.OnBelowedToZero += OnHungerZero;

        Temperature = new(100);

        Load();

        GameManager.Instance.OnSaveCallback += Save;
    }

    private void OnHungerDecrease(float amount)
    {
        if(amount < _full)
        {
            HP.regenRate = 0.0f;
        }
    }

    private void OnHungerRecevered(float amount)
    {
        if(amount >= _full)
        {
            HP.regenRate = _hpRegenOfFullState;
        }
        HP.decayRate = 0;
    }

    private void OnHungerZero()
    {
        HP.decayRate = 2.0f;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsRunning) return;

        HP.Update();
        Hunger.Update();
        UpdateTemperature();
    }

    public void SetTemperature(float temperature)
    {
        Temperature.currentValue = temperature;
        Temperature.OnUpdated?.Invoke(Temperature.GetPercentage());
    }

    private void UpdateTemperature()
    {
        float temperature = GameManager.Temperature.GetTemperature(Player.transform.position);
        SetTemperature(temperature);
    }

    //private void Load()
    //{
    //    SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, $"PlayerConditions");
    //}

    //private void Save()
    //{
    //    var json = JsonUtility.ToJson(this);
    //    SaveGame.CreateJsonFile($"PlayerConditions", json, SaveGame.SaveType.Runtime);
    //}

    private void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGameUsingFirebase.SaveToFirebase("PlayerConditions", json);
    }

    private void Load()
    {
        SaveGameUsingFirebase.LoadFromFirebase<PlayerConditionHandler>("PlayerConditions", data => {
            if (data != null)
            {
                Debug.Log($"허기:{data.Hunger.currentValue}");
                HP.currentValue = data.HP.currentValue;
                Hunger.currentValue = data.Hunger.currentValue;
                Temperature.currentValue = data.Temperature.currentValue;
            }
        });
    }
}
