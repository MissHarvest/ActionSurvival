using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 15 Park Jun Uk
public class AnimalIdleState : AnimalBaseState
{
    private Coroutine _changeStateCoroutine;
    public AnimalIdleState(AnimalStateMachine stateMachine) :base(stateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0.0f;
        base.Enter();
        StartAnimation(_stateMachine.Animal.AnimationData.IdleParameterHash);
        _changeStateCoroutine = CoroutineManagement.Instance.StartCoroutine(MoveAfterSec(3.0f));
    }

    public override void Exit()
    {
        base.Exit();
        CoroutineManagement.Instance.StopCoroutine(_changeStateCoroutine);
        _changeStateCoroutine = null;
        StopAnimation(_stateMachine.Animal.AnimationData.IdleParameterHash);
    }

    IEnumerator MoveAfterSec(float sec)
    {
        yield return new WaitForSeconds(sec);
        _stateMachine.ChangeState(_stateMachine.PatrolState);
    }
}
