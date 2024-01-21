using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
        _stateMachine.Monster.NavMeshAgent.velocity = Vector3.zero;
        _isAttacking = false;

        base.Enter();
        StartAnimation(_stateMachine.Monster.AnimationData.AttackParameterHash);
    }

    public override void Exit()
    {
        
        base.Exit();
        StopAnimation(_stateMachine.Monster.AnimationData.AttackParameterHash);
    }

    public override void Update()
    {
        if (_isAttacking == false)
        {
            Rotate();
        }

        float normalizedTime = GetNormalizedTime(_stateMachine.Monster.Animator, "Attack");
        if (normalizedTime >= 1.0f)
        {
            _stateMachine.Monster.OffAttack();
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
        else if(normalizedTime >= _stateMachine.Monster.attackTime && !_isAttacking)
        {
            _isAttacking = true;
            _stateMachine.Monster.TryAttack();
        }
    }

    private void Rotate()
    {
        var look = Managers.Game.Player.transform.position - _stateMachine.Monster.transform.position;
        look.y = 0;

        var targetRotation = Quaternion.LookRotation(look);
        _stateMachine.Monster.transform.rotation = targetRotation;
    }
}
