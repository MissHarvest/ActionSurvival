using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 15 Park Jun Uk
public class MonsterDieState : MonsterBaseState
{
    public MonsterDieState(MonsterStateMachine monsterStateMachine) : base(monsterStateMachine)
    {

    }

    public override void Enter()
    {
        Debug.Log("Monster State Changed to [ Die ]");
        _stateMachine.MovementSpeedModifier = 0.0f;
        base.Enter();
        _stateMachine.Monster.Animator.SetTrigger(_stateMachine.Monster.AnimationData.DieParameterHash);
    }

    public override void Exit() 
    {
        base.Exit();
        StopAnimation(_stateMachine.Monster.AnimationData.DieParameterHash);
    }
}
