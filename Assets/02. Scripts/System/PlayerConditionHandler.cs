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

        HP = new Condition(100);

        Hunger = new Condition(100);
        Hunger.decayRate = 0.1f;
        Hunger.OnRecovered += OnHungerRecevered;
        Hunger.OnDecreased += OnHungerDecrease;
    }

    public void OnHungerDecrease(float amount)
    {
        if(Hunger.GetPercentage() < 0.7f && _isFull)
        {
            _isFull = false;
            HP.regenRate -= 5;
        }
    }

    public void OnHungerRecevered(float amout)
    {
        if(Hunger.GetPercentage() > 0.7f &&!_isFull)
        {
            _isFull = true;
            HP.regenRate += 5;
        }
    }

    private void Update()
    {
        HP.Update();
        Hunger.Update();
    }
}
