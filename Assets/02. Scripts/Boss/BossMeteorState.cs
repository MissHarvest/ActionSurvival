using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMeteorState : BossAttackState
{
    private enum State
    {
        TakeOff,
        Fly,
        Land,
    }

    private State _currentState = State.TakeOff;

    public BossMeteorState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 100.0f;
    }

    public override void Enter()
    {
        base.Enter();
        _currentState = State.TakeOff;
        StartAnimation(_stateMachine.Boss.AnimationData.FlyParameterHash);
    }

    public override void Exit()
    {
        base.Exit();        
    }

    public override void Update()
    {
        float normalizedTime;
        switch (_currentState)
        {
            case State.TakeOff:
                normalizedTime = GetNormalizedTime(_stateMachine.Boss.Animator, "TakeOff");
                _stateMachine.Boss.NavMeshAgent.baseOffset = normalizedTime * 2.0f;
                if(normalizedTime >= 0.8f)
                {
                    ChangeState(State.Fly);
                }
                break;

            case State.Land:
                normalizedTime = GetNormalizedTime(_stateMachine.Boss.Animator, "Land");
                _stateMachine.Boss.NavMeshAgent.baseOffset = 2.0f * (1 - normalizedTime);
                if (normalizedTime >= 0.8f)
                {
                    _stateMachine.Boss.NavMeshAgent.baseOffset = 0.0f;
                    _stateMachine.ChangeState(_stateMachine.BattleState);
                }
                break;
        }
    }

    private void ChangeState(State state)
    {
        _currentState = state;

        switch (_currentState)
        {
            case State.TakeOff:
                
                break;

            case State.Fly:
                _stateMachine.Boss.NavMeshAgent.baseOffset = 2.0f;

                // Meteor - Coroutine //
                CoroutineManagement.Instance.StartCoroutine(FallMeteor());
                //
                Debug.Log("Fly");
                
                break;

            case State.Land:
                StopAnimation(_stateMachine.Boss.AnimationData.FlyParameterHash);
                break;
        }
    }

    IEnumerator FallMeteor()
    {
        yield return new WaitForSeconds(10.0f);
        ChangeState(State.Land);
    }
}
