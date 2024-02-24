using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 22 Park Jun Uk
public class MonsterStayState : MonsterBaseState
{
    private float _attackTimer = 0.0f;
    private float _reach;

    public MonsterStayState(MonsterStateMachine monsterStateMachine) : base(monsterStateMachine)
    {
        _reach = monsterStateMachine.Monster.Data.AttackData.AttackalbeDistance * 2.0f;
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0.0f;
        _stateMachine.Monster.NavMeshAgent.velocity = Vector3.zero;
        _attackTimer = 0.0f;
        base.Enter();
        StartAnimation(_stateMachine.Monster.AnimationData.IdleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _stateMachine.canAttack = true;
        StopAnimation(_stateMachine.Monster.AnimationData.IdleParameterHash);
    }

    public override void Update()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _stateMachine.Monster.Data.AttackData.AttackInterval)
        {
            _stateMachine.ChangeState(_stateMachine.ChaseState);
        }

        var sqrLength = GetDistanceBySqr(_stateMachine.Target.transform.position);

        if (sqrLength > _reach * _reach)
        {
            _stateMachine.ChangeState(_stateMachine.ChaseState);
        }
    }
}
