using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDieState : AnimalBaseState
{
    public AnimalDieState(AnimalStateMachine stateMachine) : base(stateMachine)
    {

    }
    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0.0f;
        base.Enter();
        _stateMachine.Animal.Animator.SetTrigger(_stateMachine.Animal.AnimationData.DieParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
