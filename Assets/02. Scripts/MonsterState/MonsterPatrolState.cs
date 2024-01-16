// �ۼ� ��¥ : 2024. 01. 12
// �ۼ��� : Park Jun Uk

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPatrolState : MonsterBaseState
{
    private Vector3 _destination;
    private float _recentDist = 0.0f;

    public MonsterPatrolState(MonsterStateMachine monsterStateMachine) : base (monsterStateMachine)
    {

    }

    public override void Enter()
    {
        Debug.Log("Monster State Changed to [ Patrol ]");
        _stateMachine.MovementSpeedModifier = _stateMachine.Monster.Data.MovementData.WalkSpeedModifier;
        
        base.Enter();

        SetRandomDestination();
        _stateMachine.Monster.NavMeshAgent.SetDestination(_destination);

        StartAnimation(_stateMachine.Monster.AnimationData.WalkParameterHash);
    }

    public override void Exit() 
    {
        base.Exit();

        StopAnimation(_stateMachine.Monster.AnimationData.WalkParameterHash);
    }

    public override void Update()
    {
        base.Update();

        var sqrLength = GetDistanceBySqr(_destination);        
        var stopDist = _stateMachine.Monster.NavMeshAgent.stoppingDistance;
        
        if (_recentDist == sqrLength)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }

        if (sqrLength < stopDist * stopDist)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
    }

    private void SetRandomDestination()
    {
        float maxRadius = _stateMachine.Monster.Data.MovementData.PatrolRadius;

        Vector3 direction = new Vector3(Random.Range(-maxRadius, maxRadius), 0, Random.Range(-maxRadius, maxRadius));

        direction.Normalize();

        direction *= Random.Range(1.0f, maxRadius);
        
        _destination = _stateMachine.Monster.RespawnPosition + direction;
    }
}
