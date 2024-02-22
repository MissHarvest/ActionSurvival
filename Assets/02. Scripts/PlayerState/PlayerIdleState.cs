using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    private int _targetParameterHash;

    public PlayerIdleState(PlayerStateMachine playerStateMachine) :base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0f;
        base.Enter();

        WeaponItemData hand = _stateMachine.Player.ToolSystem.EquippedTool.itemSlot.itemData as WeaponItemData;

        if (hand != null)
        {
            if (hand.isTwoHandedTool)
                _targetParameterHash = _stateMachine.Player.AnimationData.EquipTwoHandedToolIdleParameterHash;
            else if (hand.isTwinTool)
                _targetParameterHash = _stateMachine.Player.AnimationData.EquipTwinToolIdleParameterHash;
            else
                _targetParameterHash = _stateMachine.Player.AnimationData.IdleParameterHash;
        }
        else
            _targetParameterHash = _stateMachine.Player.AnimationData.IdleParameterHash;

        StartAnimation(_targetParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_targetParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (_stateMachine.MovementInput != Vector2.zero)
            OnMove();
    }
}
