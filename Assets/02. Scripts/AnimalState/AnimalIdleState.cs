using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalIdleState : AnimalBaseState
{
    public AnimalIdleState(AnimalStateMachine stateMachine) :base(stateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0.0f;
        base.Enter();
        StartAnimation(_stateMachine.Animal.AnimationData.IdleParameterHash);
        CoroutineManagement.Instance.StartCoroutine(MoveAfterSec(3.0f));
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Animal.AnimationData.IdleParameterHash);
    }

    IEnumerator MoveAfterSec(float sec)
    {
        yield return new WaitForSeconds(sec);
        if (_stateMachine.Animal.Dead == false)
            _stateMachine.ChangeState(_stateMachine.PatrolState);
    }
}
