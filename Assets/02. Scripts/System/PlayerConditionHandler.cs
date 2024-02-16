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

        Managers.Game.OnSaveCallback += Save;
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

            Managers.UI.ShowPopupUI<UIWarning>().SetWarning(
            "포만감 상태일 때,\n체력이 점차 회복됩니다.",
            UIWarning.Type.YesOnly,
            () => { Managers.UI.ClosePopupUI(); },
            true);
        }
        HP.decayRate = 0;
    }

    private void OnHungerZero()
    {
        HP.decayRate = 2.0f;

        Managers.UI.ShowPopupUI<UIWarning>().SetWarning(
            "허기가 0 입니다!!\n서둘러 음식을 섭취하세요.\n체력이 감소합니다.",
            UIWarning.Type.YesOnly,
            () => { Managers.UI.ClosePopupUI(); },
            true);
    }

    private void Update()
    {
        if (!Managers.Game.IsRunning) return;

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
        float temperature = Managers.Game.Temperature.GetTemperature(Player.transform.position);
        SetTemperature(temperature);
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
