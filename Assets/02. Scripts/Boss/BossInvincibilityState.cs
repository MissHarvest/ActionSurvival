using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossInvincibilityState : BossBaseState
{
    public BossInvincibilityState(BossStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.isBattaleState = false;
        _stateMachine.MovementSpeedModifier = _stateMachine.Boss.Data.MovementData.WalkSpeedModifier;
        _stateMachine.Boss.gameObject.layer = 14;
        _stateMachine.Boss.HP.regenRate = _stateMachine.Boss.HP.maxValue * 0.1f;
        base.Enter();
        StartAnimation(_stateMachine.Boss.AnimationData.InvincibilityParameterHash);
        _stateMachine.Boss.NavMeshAgent.SetDestination(_stateMachine.Boss.RespawnPoint); 
    }

    public override void Exit() 
    {
        base.Exit();
        StopAnimation(_stateMachine.Boss.AnimationData.InvincibilityParameterHash);
    }

    public override void Update()
    {
        if(_stateMachine.Boss.NavMeshAgent.remainingDistance == 0)
        {
            _stateMachine.ChangeState(_stateMachine.SleepState);
        }
    }
}
