using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTwinToolWalkState : PlayerGroundedState
{
    public PlayerTwinToolWalkState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }
    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = _groundData.WalkSpeedModifier;

        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.EquipTwinToolWalkParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Player.AnimationData.EquipTwinToolWalkParameterHash);
    }
    protected override void OnRunStarted(InputAction.CallbackContext context)
    {
        base.OnRunStarted(context);
        _stateMachine.ChangeState(_stateMachine.TwinToolRunState);
    }
}
