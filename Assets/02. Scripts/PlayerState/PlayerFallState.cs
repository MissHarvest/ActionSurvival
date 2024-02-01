using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerGroundedState
{
    public PlayerFallState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        StartAnimation(_stateMachine.Player.AnimationData.fallParameterHash);
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(_stateMachine.Player.AnimationData.fallParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (_stateMachine.Player.Controller.isGrounded)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }
    }
}