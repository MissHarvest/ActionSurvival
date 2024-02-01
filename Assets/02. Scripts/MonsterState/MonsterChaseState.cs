// 작성 날짜 : 2024. 01. 12
// 작성자 : Park Jun Uk

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseState : MonsterBaseState
{
    private float _reach;
    private float _remainDist;
    private GameObject _target;

    public MonsterChaseState(MonsterStateMachine monsterStateMachine) : base(monsterStateMachine)
    {
        _reach = monsterStateMachine.Monster.Data.AttackData.AttackalbeDistance;
    }

    public override void Enter()
    {   
        // Speed Up
        _stateMachine.MovementSpeedModifier = _stateMachine.Monster.Data.MovementData.ChaseSpeedModifier;
        
        // 탐지 범위 증가
        _stateMachine.DetectionDistModifier 
            = _stateMachine.Monster.Berserk ? 300 : _stateMachine.Monster.Data.AttackData.ChaseDectionDistModifier;
        
        base.Enter();
        _target = Managers.Game.Player.gameObject;
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
        var sqrLength = GetDistanceBySqr(_target.transform.position);
        
        if(sqrLength < _reach * _reach)
        {
            if(_stateMachine.canAttack)
            {
                _stateMachine.ChangeState(_stateMachine.AttackState);
                return;
            }
            _stateMachine.ChangeState(_stateMachine.StayState);
            return;
        }
        else if (TryDetectPlayer())
        {
            _stateMachine.Monster.NavMeshAgent.SetDestination(Managers.Game.Player.transform.position);
            
            // [ Error ]
            //var dist = _stateMachine.Monster.NavMeshAgent.remainingDistance;
            //if(dist == _remainDist)
            //{

            //}
            //else
            //{
            //    _remainDist = dist;

            //}            
            return;
        }
        else
        {
            _stateMachine.ChangeState(_stateMachine.PatrolState);
            return;
        }
    }

    private void Test()
    {
        // Player 대신 다른 타겟을 찾는다.

        // 
    }
}
