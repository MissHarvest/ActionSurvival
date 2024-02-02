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
        // Speed Up
        _stateMachine.MovementSpeedModifier = _stateMachine.Monster.Data.MovementData.ChaseSpeedModifier;
        
        // 탐지 범위 증가
        _stateMachine.DetectionDistModifier 
            = _stateMachine.Monster.Berserk ? 300 : _stateMachine.Monster.Data.AttackData.ChaseDectionDistModifier;
        
        base.Enter();
        _stateMachine.Target = _stateMachine.Target == null ? Managers.Game.Player.gameObject : _stateMachine.Target;
        _stateMachine.Monster.NavMeshAgent.SetDestination(_stateMachine.Target.transform.position);
        if (_stateMachine.Monster.NavMeshAgent.remainingDistance == 0)
        {
            RetargetingToArchitecture();
        }

        StartAnimation(_stateMachine.Monster.AnimationData.RunParameterHash);
    }

    public override void Exit()
    {        
        base.Exit();

        StopAnimation(_stateMachine.Monster.AnimationData.RunParameterHash);
    }

    public override void Update()
    {
        if (_stateMachine.Target == null) return;
        Debug.Log($"[{_stateMachine.Monster.name}] target[{_stateMachine.Target.name}] [{_stateMachine.Monster.NavMeshAgent.remainingDistance}]");
        var sqrLength = GetDistanceBySqr(_stateMachine.Target.transform.position);
        
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

        if(_stateMachine.Target == Managers.Game.Player.gameObject)
            _stateMachine.Monster.NavMeshAgent.SetDestination(_stateMachine.Target.transform.position);
    }

    private void RetargetingToArchitecture()
    {
        Vector3 dir = Managers.Game.Player.transform.position - _stateMachine.Monster.transform.position;
        dir.y = 0;

        RaycastHit hit;
        if(Physics.Raycast(_stateMachine.Monster.transform.position + Vector3.up,
            dir, out hit, dir.magnitude, 1 << 11))
        {
            _stateMachine.Target = hit.collider.gameObject;
            _stateMachine.Monster.NavMeshAgent.SetDestination(_stateMachine.Target.transform.position);
        }
    }
}
