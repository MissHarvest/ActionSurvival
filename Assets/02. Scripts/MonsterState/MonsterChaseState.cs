// 작성 날짜 : 2024. 01. 12
// 작성자 : Park Jun Uk

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseState : MonsterBaseState
{
    public MonsterChaseState(MonsterStateMachine monsterStateMachine) : base(monsterStateMachine)
    {

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

        // Exit Animation
        StopAnimation(_stateMachine.Monster.AnimationData.RunParameterHash);
    }

    public override void Update()
    {
        // base.Update();

        var sqrLength = GetDistanceBySqr(Managers.Game.Player.transform.position);
        var reach = _stateMachine.Monster.Data.AttackData.AttackalbeDistance;

        if(sqrLength < reach * reach)
        {
            _stateMachine.ChangeState(_stateMachine.AttackState);
        }
        else if (TryDetectPlayer())
        {
            _stateMachine.Monster.NavMeshAgent.SetDestination(Managers.Game.Player.transform.position);
        }
        else
        {
            _stateMachine.ChangeState(_stateMachine.PatrolState);
        }
    }
}
