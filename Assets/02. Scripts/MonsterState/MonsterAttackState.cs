using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : MonsterBaseState
{
    private bool _isAttacking;
    public MonsterAttackState(MonsterStateMachine monsterStateMachine) : base(monsterStateMachine)
    {

    }

    public override void Enter()
    {
        Debug.Log("Monster State Changed to [ Attack ]");
        _stateMachine.MovementSpeedModifier = 0.0f;
        base.Enter();
        StartAnimation(_stateMachine.Monster.AnimationData.AttackParameterHash);
        //_stateMachine.Monster.NavMeshAgent.isStopped = true;
    }

    public override void Exit()
    {
        
        base.Exit();
        StopAnimation(_stateMachine.Monster.AnimationData.AttackParameterHash);
        //_stateMachine.Monster.NavMeshAgent.isStopped = false;
    }

    public override void Update()
    {
        // base.Update();        

        float normalizedTime = GetNormalizedTime(_stateMachine.Monster.Animator, "Attack");
        if (normalizedTime >= 1.0f)
        {
            if (TryDetectPlayer())
            {
                _stateMachine.ChangeState(_stateMachine.ChaseState);
                return;
            }
            else
            {
                _stateMachine.ChangeState(_stateMachine.PatrolState);
            }
        }
    }
}
