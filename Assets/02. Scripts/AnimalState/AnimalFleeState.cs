using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 15 Park Jun Uk
public class AnimalFleeState : AnimalBaseState
{
    private float _recentDist = 0.0f;
    public AnimalFleeState(AnimalStateMachine stateMachine) : base(stateMachine)
    {

    }
    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = _stateMachine.Animal.Data.MovementData.FleeSpeedModifier;
        base.Enter();
        Flee();
        StartAnimation(_stateMachine.Animal.AnimationData.RunParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Animal.AnimationData.RunParameterHash);
    }

    public override void Update()
    {
        base.Update();

        var sqrLength = GetDistanceBySqr(_stateMachine.Animal.RespawnPosition);
        var stopDist = _stateMachine.Animal.NavMeshAgent.stoppingDistance;

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

    private void Flee()
    {
        // 나의 위치 - 플레이어 위치 = 도망갈 방향
        var fleeDirection = _stateMachine.Animal.transform.position - _stateMachine.Attacker.transform.position;
        fleeDirection.y = 0;
        fleeDirection.Normalize();

        // 도망 거리
        fleeDirection *= Random.Range(5.0f, 15.0f);

        _stateMachine.Animal.RespawnPosition = _stateMachine.Animal.transform.position + fleeDirection;
        // Set Destination
        _stateMachine.Animal.NavMeshAgent.SetDestination(_stateMachine.Animal.RespawnPosition);
    }
}
