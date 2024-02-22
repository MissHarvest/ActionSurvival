using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        if (_stateMachine.MovementInput == Vector2.zero)
        {
            ChangeIdleState();
            return;
        }

        if (_stateMachine.IsAttacking)
        {
            OnAttack();
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        bool isHit = Physics.SphereCast(GameManager.Instance.Player.ViewPoint.transform.position, 0.5f, Vector3.down, out RaycastHit hit, 1f);

        if (!isHit && _stateMachine.Player.Controller.velocity.y < Physics.gravity.y * Time.fixedDeltaTime)
        {
            _stateMachine.ChangeState(_stateMachine.FallState);
        }
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        _stateMachine.Player.ToolSystem.OnEquip += OnEquipTypeOfTool;
        //Managers.Game.Player.ToolSystem.OnUnEquip += OnUnEquipTypeOfTool;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        _stateMachine.Player.ToolSystem.OnEquip -= OnEquipTypeOfTool;
        //Managers.Game.Player.ToolSystem.OnUnEquip -= OnUnEquipTypeOfTool;
    }

    protected virtual void OnMove()
    {
        _stateMachine.ChangeState(_stateMachine.RunState);
    }

    protected virtual void OnAttack()
    {
        _stateMachine.ChangeState(_stateMachine.ComboAttackState);
    }

    protected virtual void OnEquipTypeOfTool(QuickSlot quickSlot)
    {        
        ChangeIdleState();
    }

    protected void ChangeIdleState()
    {
        _stateMachine.ChangeState(_stateMachine.IdleState);
    }

    protected virtual void OnUnEquipTypeOfTool(QuickSlot quickSlot)
    {
        _stateMachine.ChangeState(_stateMachine.IdleState);
    }
}
