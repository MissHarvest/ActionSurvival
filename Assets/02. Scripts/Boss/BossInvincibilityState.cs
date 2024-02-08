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
        _stateMachine.Boss.gameObject.layer *= 2;
        _stateMachine.Boss.HP.regenRate = _stateMachine.Boss.HP.maxValue * 0.1f;
        base.Enter();
        _stateMachine.Boss.NavMeshAgent.SetDestination(_stateMachine.Boss.RespawnPoint);
        StopAnimation(_stateMachine.Boss.AnimationData.BattleParamterHash);
    }

    public override void Exit() 
    {
        base.Exit();
    }

    public override void Update()
    {
        if(_stateMachine.Boss.NavMeshAgent.remainingDistance == 0)
        {
            _stateMachine.ChangeState(_stateMachine.SleepState);
        }
    }
}
