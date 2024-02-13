using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdleState : BossBaseState
{
    public BossIdleState(BossStateMachine stateMachine) :base(stateMachine)
    {

    }

    public override void Enter()
    {
        // Set Speed zero
        _stateMachine.MovementSpeedModifier = 0.0f;
        base.Enter();
        // Start Animation
        StartAnimation(_stateMachine.Boss.AnimationData.IdleParameterHash);
    }

    public override void Exit() 
    {
        base.Exit();
        StopAnimation(_stateMachine.Boss.AnimationData.IdleParameterHash);
    }

    public override void Update()
    {
        if(_stateMachine.NextAttackState != null)
        {
            _stateMachine.ChangeState(_stateMachine.BattleState);
            return;
        }
    }
}
