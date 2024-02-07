using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBiteState : BossAttackState
{
    public BossBiteState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 5.0f;
        cooltime = 3.0f;
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0.0f;
        _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Head).ActivateWeapon();
        base.Enter();
        StartAnimation(_stateMachine.Boss.AnimationData.BiteParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Head).InactivateWeapon();
        StopAnimation(_stateMachine.Boss.AnimationData.BiteParameterHash);
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
