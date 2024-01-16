using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkState : PlayerGroundedState
{
    public PlayerWalkState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        //ToolItemData toolItemDate = (ToolItemData)Managers.Game.Player.ToolSystem.ItemInUse.itemData;

        _stateMachine.MovementSpeedModifier = _groundData.WalkSpeedModifier;

        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.WalkParameterHash);
        StartAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolWalkParameterHash);
        //Debug.Log(toolItemDate.name);

        //if (toolItemDate.isTwoHandedTool == true)
        //{
        //    StartAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolWalkParameterHash);
        //}
        //else
        //{
        //    StartAnimation(_stateMachine.Player.AnimationData.WalkParameterHash);
        //}
    }

    public override void Exit()
    {
        //ToolItemData toolItemDate = (ToolItemData)Managers.Game.Player.ToolSystem.ItemInUse.itemData;

        base.Exit();
        StopAnimation(_stateMachine.Player.AnimationData.WalkParameterHash);
        StopAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolWalkParameterHash);

        //if (toolItemDate.isTwoHandedTool == true)
        //{
        //    StopAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolWalkParameterHash);
        //}
        //else
        //{
        //    StopAnimation(_stateMachine.Player.AnimationData.WalkParameterHash);
        //}
    }

    protected override void OnRunStarted(InputAction.CallbackContext context)
    {
        base.OnRunStarted(context);
        _stateMachine.ChangeState(_stateMachine.RunState);
        Debug.Log("Player Run");
    }
}
