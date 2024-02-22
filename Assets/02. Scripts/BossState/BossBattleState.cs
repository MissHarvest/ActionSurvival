using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleState : BossBaseState
{
    public BossBattleState(BossStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = _stateMachine.Boss.Data.MovementData.WalkSpeedModifier;
        base.Enter();
        StartAnimation(_stateMachine.Boss.AnimationData.BattleParamterHash);
    }

    public override void Exit() 
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (!_stateMachine.isBattaleState) return;

        if (_stateMachine.NextAttackState == null)
        {
            StopAnimation(_stateMachine.Boss.AnimationData.BattleParamterHash);
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }

        RotateToTarget();

        // Move
        _stateMachine.Boss.NavMeshAgent.SetDestination(_stateMachine.Target.transform.position);

        if (!CheckTargetInViewArea() || !CheckTargetInReach()) return;
        
        _stateMachine.ChangeState(_stateMachine.NextAttackState);
    }

    private bool CheckTargetInReach()
    {
        var reach = _stateMachine.NextAttackState != null ? _stateMachine.NextAttackState._reach : 5.0f;
        return GetDistToTarget() <= (reach * reach);
    }
}
