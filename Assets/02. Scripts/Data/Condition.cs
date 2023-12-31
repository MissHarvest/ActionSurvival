using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

[System.Serializable]
public class Condition
{
    public float currentValue;
    public float maxValue;
    public float regenRate;
    public float decayRate;

    public Action<float> OnRecovered;
    public Action<float> OnDecreased;
    public Action<float> OnUpdated;
    public Action OnBelowedToZero;

    public Condition(float maxValue)
    {
        this.maxValue = maxValue;
        this.currentValue = this.maxValue;
    }

    public void Add(float amount)
    {
        if (currentValue >= maxValue) return;

        currentValue = Mathf.Min(currentValue + amount, maxValue);
        OnUpdated?.Invoke(GetPercentage());
    }

    public void Subtract(float amount)
    {
        if (currentValue == 0) return;

        currentValue = Mathf.Max(currentValue - amount, 0.0f);
        OnUpdated?.Invoke(GetPercentage());
        if(currentValue == 0)
        {
            OnBelowedToZero?.Invoke();
        }
    }

    public float GetPercentage()
    {
        return currentValue / maxValue;
    }

    public void Update()
    {
        Add(regenRate * Time.deltaTime);
        Subtract(decayRate * Time.deltaTime);
    }
}
