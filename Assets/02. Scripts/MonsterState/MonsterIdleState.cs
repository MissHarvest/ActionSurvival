// 작성 날짜 : 2024. 01. 12
// 작성자 : Park Jun Uk

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleState : MonsterBaseState
{
    private Coroutine _changeStateCoroutine;

    public MonsterIdleState(MonsterStateMachine monsterStateMachine) : base(monsterStateMachine)
    {
        
    }

    public override void Enter()
    {
        Debug.Log("Monster State Changed to [ Idle ]");
        // Set Speed zero
        _stateMachine.MovementSpeedModifier = 0.0f;
        base.Enter();

        // Start Animation
        StartAnimation(_stateMachine.Monster.AnimationData.IdleParameterHash);
        _changeStateCoroutine = CoroutineManagement.Instance.StartCoroutine(MoveAfterSec(3.0f));
    }

    public override void Exit()
    {
        base.Exit();
        CoroutineManagement.Instance.StopCoroutine(_changeStateCoroutine);
        _changeStateCoroutine = null;
        StopAnimation(_stateMachine.Monster.AnimationData.IdleParameterHash);
    }

    IEnumerator MoveAfterSec(float sec)
    {
        yield return new WaitForSeconds(sec);
        if(_stateMachine.Monster.Dead == false)
            _stateMachine.ChangeState(_stateMachine.PatrolState);
    }
}
