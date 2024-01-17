using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTwoHandedToolWalkState : PlayerGroundedState
{
    public PlayerTwoHandedToolWalkState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }
    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = _groundData.WalkSpeedModifier;

        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolWalkParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolWalkParameterHash);
    }
    protected override void OnRunStarted(InputAction.CallbackContext context)
    {
        base.OnRunStarted(context);
        _stateMachine.ChangeState(_stateMachine.TwoHandedToolRunState);
    }
}
