using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerGroundedState
{
    private bool _alreadyAppliedForce;
    public PlayerFallState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _stateMachine.IsFalling = true;
        _stateMachine.MovementSpeedModifier = 0f;        
        _alreadyAppliedForce = false;
        StartAnimation(_stateMachine.Player.AnimationData.fallParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _stateMachine.IsFalling = false;
        StopAnimation(_stateMachine.Player.AnimationData.fallParameterHash);
    }

    public override void Update()
    {
        base.Update();
        ToolItemData toolItemDate = (ToolItemData)Managers.Game.Player.ToolSystem.ItemInUse.itemData;

        TryApplyForce();

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

    private void TryApplyForce()
    {
        if (_alreadyAppliedForce) return;
        _alreadyAppliedForce = true;

        _stateMachine.Player.ForceReceiver.AddForce(_stateMachine.Player.transform.forward * 0.2f);
    }
}