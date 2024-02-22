using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDieState : BossBaseState
{
    public BossDieState(BossStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0.0f;
        base.Enter();
        _stateMachine.Boss.Animator.SetTrigger(_stateMachine.Boss.AnimationData.DieParameterHash);
    }

    public override void Update()
    {
        float normalizedTime = GetNormalizedTime(_stateMachine.Boss.Animator, "Die");
        if (normalizedTime >= 1.0f)
        {
            _stateMachine.Boss.gameObject.SetActive(false);
        }
    }
}
