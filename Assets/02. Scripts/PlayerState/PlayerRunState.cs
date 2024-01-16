using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunState : PlayerGroundedState
{
    public PlayerRunState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        //ToolItemData toolItemDate = (ToolItemData)Managers.Game.Player.ToolSystem.ItemInUse.itemData;
        _stateMachine.MovementSpeedModifier = _groundData.RunSpeedModifier;
        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.RunParameterHash);
        StartAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolRunParameterHash);

        //if (toolItemDate.isTwoHandedTool == true)
        //{
        //    StartAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolRunParameterHash);
        //}
        //else
        //{
        //    StartAnimation(_stateMachine.Player.AnimationData.RunParameterHash);
        //}
    }

    public override void Exit()
    {
        //ToolItemData toolItemDate = (ToolItemData)Managers.Game.Player.ToolSystem.ItemInUse.itemData;
        base.Exit();
        StopAnimation(_stateMachine.Player.AnimationData.RunParameterHash);
        StopAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolRunParameterHash);

        //if (toolItemDate.isTwoHandedTool == true)
        //{
        //    StopAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolRunParameterHash);
        //}
        //else
        //{
        //    StopAnimation(_stateMachine.Player.AnimationData.RunParameterHash);
        //}
    }

    protected override void OnRunCanceled(InputAction.CallbackContext context)
    {
        base.OnRunCanceled(context);
        _stateMachine.ChangeState(_stateMachine.WalkState);
    }
}
