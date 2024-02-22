using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunState : PlayerGroundedState
{
    private int _targetParameterHash;

    public PlayerRunState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = _groundData.RunSpeedModifier;
        base.Enter();

        WeaponItemData hand = _stateMachine.Player.ToolSystem.EquippedTool.itemSlot.itemData as WeaponItemData;

        if (hand != null)
        {
            if (hand.isTwoHandedTool)
                _targetParameterHash = _stateMachine.Player.AnimationData.EquipTwoHandedToolRunParameterHash;
            else if (hand.isTwinTool)
                _targetParameterHash = _stateMachine.Player.AnimationData.EquipTwinToolRunParameterHash;
            else
                _targetParameterHash = _stateMachine.Player.AnimationData.RunParameterHash;
        }
        else
            _targetParameterHash = _stateMachine.Player.AnimationData.RunParameterHash;

        StartAnimation(_targetParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_targetParameterHash);
    }

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        base.OnMovementCanceled(context);
        _stateMachine.ChangeState(_stateMachine.IdleState);
    }
}
