using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.GroundParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Player.AnimationData.GroundParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if(_stateMachine.IsAttacking)
        {
            OnAttack();
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if(!_stateMachine.Player.Controller.isGrounded
            && _stateMachine.Player.Controller.velocity.y < Physics.gravity.y * Time.fixedDeltaTime)
        {
            //_stateMachine.ChangeState(_stateMachine.FallState);
        }
    }

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        if(_stateMachine.MovementInput == Vector2.zero)
        {
            return;
        }

        _stateMachine.ChangeState(_stateMachine.IdleState);

        base.OnMovementCanceled(context);
    }

    protected virtual void OnMove()
    {
        _stateMachine.ChangeState(_stateMachine.WalkState);
    }
    protected virtual void OnAttack()
    {
        // idle는 멈추는데 TwoHanded...idle는 살아 있어서 버그가 생긴다.
        StopAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolIdleParameterHash);
        _stateMachine.ChangeState(_stateMachine.ComboAttackState);
    }
}
