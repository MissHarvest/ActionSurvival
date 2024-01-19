// 작성 날짜 : 2024. 01. 12
// 작성자 : Park Jun Uk

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseState : MonsterBaseState
{
    private float _reach;

    public MonsterChaseState(MonsterStateMachine monsterStateMachine) : base(monsterStateMachine)
    {
        _reach = monsterStateMachine.Monster.Data.AttackData.AttackalbeDistance;
    }

    public override void Enter()
    {
        Debug.Log("Monster State Changed to [ Chase ]");
        // Speed Up
        _stateMachine.MovementSpeedModifier = _stateMachine.Monster.Data.MovementData.ChaseSpeedModifier;
        // 탐지 범위 증가
        _stateMachine.DetectionDistModifier = _stateMachine.Monster.Data.AttackData.ChaseDectionDistModifier;
        base.Enter();
        
        // Start Animation
        StartAnimation(_stateMachine.Monster.AnimationData.RunParameterHash);
    }

    public override void Exit()
    {        
        base.Exit();

        // 탐지 범위 감소
        _stateMachine.DetectionDistModifier = _stateMachine.Monster.Data.AttackData.DefaultDetectionDistModifier;

        _stateMachine.Monster.NavMeshAgent.stoppingDistance = 0.5f;
        // Exit Animation
        StopAnimation(_stateMachine.Monster.AnimationData.RunParameterHash);
    }

    public override void Update()
    {
        // base.Update();

        var dist = _stateMachine.Monster.NavMeshAgent.remainingDistance;
        var maxSpeed = _stateMachine.Monster.NavMeshAgent.speed;
        dist = Mathf.Min(dist, maxSpeed);
        _stateMachine.Monster.NavMeshAgent.stoppingDistance
            = Mathf.Max((dist + _reach) * 0.5f, _stateMachine.Monster.NavMeshAgent.stoppingDistance);

        var sqrLength = GetDistanceBySqr(Managers.Game.Player.transform.position);
        
        if(sqrLength < _reach * _reach)
        {
            _stateMachine.Monster.NavMeshAgent.ResetPath();
            _stateMachine.ChangeState(_stateMachine.AttackState);
            return;
        }
        else if (TryDetectPlayer())
        {
            _stateMachine.Monster.NavMeshAgent.SetDestination(Managers.Game.Player.transform.position);
            return;
        }
        else
        {
            _stateMachine.ChangeState(_stateMachine.PatrolState);
            return;
        }
    }
}
