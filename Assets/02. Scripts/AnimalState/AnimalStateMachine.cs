using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// 2024. 01. 15 Park Jun Uk
public class AnimalStateMachine : StateMachine
{
    public Animal Animal { get; }
    public float MovementSpeed { get; private set; }
    public float MovementSpeedModifier { get; set; } = 1.0f;
    
    public AnimalIdleState IdleState { get; }
    public AnimalPatrolState PatrolState { get; }
    public AnimalFleeState FleeState { get; }
    public AnimalDieState DieState { get; }

    public GameObject Attacker { get; set; }

    public AnimalStateMachine(Animal animal)
    {
        this.Animal = animal;
        MovementSpeed = animal.Data.MovementData.BaseSpeed;

        IdleState = new AnimalIdleState(this);
        PatrolState = new AnimalPatrolState(this);
        FleeState = new AnimalFleeState(this);
        DieState = new AnimalDieState(this);
    }
}
