using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTwoHandedToolIdleState : PlayerGroundedState
{
    public PlayerTwoHandedToolIdleState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0f;
        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolIdleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolIdleParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (_stateMachine.MovementInput != Vector2.zero)
        {
            OnMove();
            return;
        }
    }
}
