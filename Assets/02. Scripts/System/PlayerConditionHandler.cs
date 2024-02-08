using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerConditionHandler : MonoBehaviour
{
    public Player Player { get; private set; }
    [field: SerializeField] public Condition HP { get; private set; }
    [field: SerializeField] public Condition Hunger { get; private set; }

    private bool _isFull;

    private void Awake()
    {
        Player = GetComponent<Player>();

        HP = new Condition(200);
        HP.regenRate = 0.1f;
        HP.OnBelowedToZero += Player.Die;

        Hunger = new Condition(100);
        Hunger.decayRate = 0.2f;
        Hunger.OnRecovered += OnHungerRecevered;
        Hunger.OnDecreased += OnHungerDecrease;
        Hunger.OnBelowedToZero += OnHungerZero;

        Load();

        Managers.Game.OnSaveCallback += Save;
    }

    private void OnHungerDecrease(float amount)
    {
        if(amount < 0.8f)
        {
            HP.regenRate = 0.0f;
        }
    }

    private void OnHungerRecevered(float amount)
    {
        if(amount > 0.8f)
        {
            HP.regenRate = 0.1f;
        }
        HP.decayRate = 0;
    }

    private void OnHungerZero()
    {
        HP.decayRate = 2.0f;
    }

    private void Update()
    {
        HP.Update();
        Hunger.Update();
    }

    private void Load()
    {
        SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, $"PlayerConditions");
    }

    private void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile($"PlayerConditions", json, SaveGame.SaveType.Runtime);
    }
}
