using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalBaseState : IState
{
    protected AnimalStateMachine _stateMachine;

    public AnimalBaseState(AnimalStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public virtual void Enter()
    {
        _stateMachine.Animal.NavMeshAgent.speed = _stateMachine.MovementSpeed * _stateMachine.MovementSpeedModifier;
    }

    public virtual void Exit()
    {

    }

    public virtual void Update()
    {
        
    }

    public virtual void HandleInput()
    {

    }

    public virtual void PhysicsUpdate()
    {

    }

    protected void StartAnimation(int animationHash)
    {
        _stateMachine.Animal.Animator.SetBool(animationHash, true);
    }

    protected void StopAnimation(int animationHash)
    {
        _stateMachine.Animal.Animator.SetBool(animationHash, false);
    }

    protected float GetDistanceBySqr(Vector3 target)
    {
        var offset = _stateMachine.Animal.transform.position - target;
        var sqrLength = offset.sqrMagnitude;
        return sqrLength;
    }
}
