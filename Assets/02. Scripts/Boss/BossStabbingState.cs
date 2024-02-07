using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStabbingState : BossAttackState
{
    public BossStabbingState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 5.05f;
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0.0f;
        _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.LeftWing).ActivateWeapon();
        _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.RightWing).ActivateWeapon();
        _stateMachine.Boss.NavMeshAgent.velocity = Vector3.zero;
        base.Enter();
        StartAnimation(_stateMachine.Boss.AnimationData.StabParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.LeftWing).InactivateWeapon();
        _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.RightWing).InactivateWeapon();
        StopAnimation(_stateMachine.Boss.AnimationData.StabParameterHash);
    }

    public override void Update()
    {
        float normalizedTime = GetNormalizedTime(_stateMachine.Boss.Animator, "Attack");
        if (normalizedTime >= 1.0f)
        {
            _stateMachine.ChangeState(_stateMachine.BattleState);
        }
    }
}
