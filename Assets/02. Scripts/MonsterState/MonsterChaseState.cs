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
        Debug.Log($"{_stateMachine.Monster.name} State Changed to [ Chase ]");
        // Speed Up
        _stateMachine.MovementSpeedModifier = _stateMachine.Monster.Data.MovementData.ChaseSpeedModifier;
        // 탐지 범위 증가
        if(_stateMachine.Monster.Berserk)
        {
            _stateMachine.DetectionDistModifier = 300;
        }
        else
        {
            _stateMachine.DetectionDistModifier = _stateMachine.Monster.Data.AttackData.ChaseDectionDistModifier;
        }
        
        base.Enter();
        
        // Start Animation
        StartAnimation(_stateMachine.Monster.AnimationData.RunParameterHash);
    }

    public override void Exit()
    {        
        base.Exit();

        StopAnimation(_stateMachine.Monster.AnimationData.RunParameterHash);
    }

    public override void Update()
    {
        // base.Update();

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
