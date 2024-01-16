// 작성 날짜 : 2024. 01. 12
// 작성자 : Park Jun Uk

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine : StateMachine
{
    public Monster Monster { get; }
    public float MovementSpeed { get; private set; }    
    public float MovementSpeedModifier { get; set; } = 1.0f;
    public float DetectionDist { get; private set; }
    public float DetectionDistModifier { get; set; } = 1.0f;

    // State Branch
    public MonsterIdleState IdleState {get;}
    public MonsterPatrolState PatrolState { get; }
    public MonsterChaseState ChaseState { get; }
    public MonsterAttackState AttackState { get; }
    public MonsterDieState DieState { get; }

    public MonsterStateMachine(Monster monster)
    {
        this.Monster = monster;
        MovementSpeed = monster.Data.MovementData.BaseSpeed;
        DetectionDist = monster.Data.AttackData.DetectionDistance;

        IdleState = new MonsterIdleState(this);
        PatrolState = new MonsterPatrolState(this);
        ChaseState = new MonsterChaseState(this);
        AttackState = new MonsterAttackState(this);
        DieState = new MonsterDieState(this);
    }    
}
