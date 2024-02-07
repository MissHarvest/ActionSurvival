using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRushState : BossAttackState
{
    public BossRushState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 30.0f;
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 3.0f;
        _stateMachine.Boss.BodyCollider.isTrigger = true;
        _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Body).ActivateWeapon();
        base.Enter();
        var direction = _stateMachine.Target.transform.position - _stateMachine.Boss.transform.position;
        direction.Normalize();
        var destination = _stateMachine.Boss.transform.position + direction * 40.0f;
        _stateMachine.Boss.NavMeshAgent.SetDestination(destination);
        StartAnimation(_stateMachine.Boss.AnimationData.RushParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Body).InactivateWeapon();
        _stateMachine.Boss.BodyCollider.isTrigger = false;
        StopAnimation(_stateMachine.Boss.AnimationData.RushParameterHash);
    }

    public override void Update()
    {
        if(_stateMachine.Boss.NavMeshAgent.remainingDistance < 0.1f)
        {
            _stateMachine.ChangeState(_stateMachine.BattleState);
        }
    }
}
