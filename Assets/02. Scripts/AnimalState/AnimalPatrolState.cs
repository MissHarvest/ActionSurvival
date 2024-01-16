using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPatrolState : AnimalBaseState
{
    private Vector3 _destination;
    private float _recentDist = 0.0f;
    public AnimalPatrolState(AnimalStateMachine stateMachine) : base(stateMachine)
    {

    }
    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = _stateMachine.Animal.Data.MovementData.WalkSpeedModifier;
        base.Enter();
        SetRandomDestination();
        _stateMachine.Animal.NavMeshAgent.SetDestination(_destination);
        StartAnimation(_stateMachine.Animal.AnimationData.WalkParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Animal.AnimationData.WalkParameterHash);
    }

    public override void Update()
    {
        base.Update();

        var sqrLength = GetDistanceBySqr(_destination);
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

    private void SetRandomDestination()
    {
        float maxRadius = _stateMachine.Animal.Data.MovementData.PatrolRadius;

        Vector3 direction = new Vector3(Random.Range(-maxRadius, maxRadius), 0, Random.Range(-maxRadius, maxRadius));

        direction.Normalize();

        direction *= Random.Range(1.0f, maxRadius);

        _destination = _stateMachine.Animal.RespawnPosition + direction;
    }
}
