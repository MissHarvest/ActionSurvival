using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTwoHandedToolRunState : PlayerGroundedState
{
    public PlayerTwoHandedToolRunState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }
    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = _groundData.RunSpeedModifier;

        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolRunParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolRunParameterHash);
    }
}
