// 작성 날짜 : 2024. 01. 12
// 작성자 : Park Jun Uk

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseState : MonsterBaseState
{
    private float _maxSpeed;
    private float _reach;

    public MonsterChaseState(MonsterStateMachine monsterStateMachine) : base(monsterStateMachine)
    {
        _maxSpeed = monsterStateMachine.Monster.NavMeshAgent.speed;
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

        // Speed Set Default


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
        dist = Mathf.Min(dist, _maxSpeed);// dist > 4.0f ? 4.0f : dist; // 4.0f  속도
        _stateMachine.Monster.NavMeshAgent.stoppingDistance
            = Mathf.Max((dist + _reach) * 0.5f, _stateMachine.Monster.NavMeshAgent.stoppingDistance); // 0.5f 기본 정지거리
        // 1.0f 은 공격 사거리?

        var sqrLength = GetDistanceBySqr(Managers.Game.Player.transform.position);
        
        if(sqrLength < _reach * _reach)
        {
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
