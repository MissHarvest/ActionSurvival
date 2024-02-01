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
        _target = _target == null ? Managers.Game.Player.gameObject : _target;
        _stateMachine.Monster.NavMeshAgent.stoppingDistance = _target == Managers.Game.Player ? 0 : _reach;
        _stateMachine.Monster.NavMeshAgent.SetDestination(_target.transform.position);
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
            if (_stateMachine.canAttack)
            {
                _stateMachine.ChangeState(_stateMachine.AttackState);
            }
            else
            {
                _stateMachine.ChangeState(_stateMachine.StayState);
            }
            return;
        }
        
        if (TryDetectPlayer() == false)
        {
            _stateMachine.ChangeState(_stateMachine.PatrolState);
            return;
        }

        // Try Detecte Player == true, sqrLength > reach
        if (_target == Managers.Game.Player) _stateMachine.Monster.NavMeshAgent.SetDestination(_target.transform.position);
        
        if(_stateMachine.Monster.NavMeshAgent.remainingDistance < _reach && _target == Managers.Game.Player.gameObject)
        {
            RetargetingToArchitecture();
        }
    }

    private void RetargetingToArchitecture()
    {
        Vector3 dir = Managers.Game.Player.transform.position - _stateMachine.Monster.transform.position;
        dir.y = 0;

        RaycastHit hit;
        if(Physics.Raycast(_stateMachine.Monster.transform.position + Vector3.up,
            dir, out hit, dir.magnitude, 1 << 11))
        {
            _target = hit.collider.gameObject;
            _stateMachine.Monster.NavMeshAgent.stoppingDistance = _reach;
            _stateMachine.Monster.NavMeshAgent.SetDestination(_target.transform.position);
        }
    }
}
