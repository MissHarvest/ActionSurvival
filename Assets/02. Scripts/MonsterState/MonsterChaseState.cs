// �ۼ� ��¥ : 2024. 01. 12
// �ۼ��� : Park Jun Uk

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
        // Ž�� ���� ����
        _stateMachine.DetectionDistModifier = _stateMachine.Monster.Data.AttackData.ChaseDectionDistModifier;
        base.Enter();
        
        // Start Animation
        StartAnimation(_stateMachine.Monster.AnimationData.RunParameterHash);
    }

    public override void Exit()
    {        
        base.Exit();

        // Speed Set Default


        // Ž�� ���� ����
        _stateMachine.DetectionDistModifier = _stateMachine.Monster.Data.AttackData.DefaultDetectionDistModifier;

        _stateMachine.Monster.NavMeshAgent.stoppingDistance = 0.5f;
        // Exit Animation
        StopAnimation(_stateMachine.Monster.AnimationData.RunParameterHash);
    }

    public override void Update()
    {
        // base.Update();

        var dist = _stateMachine.Monster.NavMeshAgent.remainingDistance;
        dist = Mathf.Min(dist, _maxSpeed);// dist > 4.0f ? 4.0f : dist; // 4.0f  �ӵ�
        _stateMachine.Monster.NavMeshAgent.stoppingDistance
            = Mathf.Max((dist + _reach) * 0.5f, _stateMachine.Monster.NavMeshAgent.stoppingDistance); // 0.5f �⺻ �����Ÿ�
        // 1.0f �� ���� ��Ÿ�?

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
