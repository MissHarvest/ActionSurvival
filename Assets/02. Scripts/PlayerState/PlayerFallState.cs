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
        ToolItemData toolItemDate = (ToolItemData)Managers.Game.Player.ToolSystem.ItemInUse.itemData;

        _stateMachine.MovementSpeedModifier -= Time.deltaTime;

        if (_stateMachine.Player.Controller.isGrounded)
        {
            if (toolItemDate.isTwoHandedTool == true)
            {
                _stateMachine.ChangeState(_stateMachine.TwoHandedToolIdleState);
            }
            else if (toolItemDate.isTwinTool == true)
            {
                _stateMachine.ChangeState(_stateMachine.TwinToolIdleState);
            }
            else
            {
                _stateMachine.ChangeState(_stateMachine.IdleState);
            }
            return;
        }
    }
    protected override void Rotate(Vector3 movementDirection)
    {

    }
}