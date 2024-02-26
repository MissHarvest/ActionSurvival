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
            _stateMachine.ChangeState(_stateMachine.IdleState);
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
        _stateMachine.Player.ToolSystem.OnEquip += OnChangedEquipTool;
        _stateMachine.Player.ToolSystem.OnUnEquip += OnChangedEquipTool;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        _stateMachine.Player.ToolSystem.OnEquip -= OnChangedEquipTool;
        _stateMachine.Player.ToolSystem.OnUnEquip -= OnChangedEquipTool;
    }

    protected virtual void OnMove()
    {
        _stateMachine.ChangeState(_stateMachine.RunState);
    }

    protected virtual void OnChangedEquipTool(QuickSlot quickSlot) 
    {
        WeaponItemData hand = quickSlot.itemSlot.itemData as WeaponItemData;

        if (hand != null)
        {
            _stateMachine.Player.Animator.SetBool(_stateMachine.Player.AnimationData.EquipTwoHandedToolParameterHash, hand.isTwoHandedTool && quickSlot.itemSlot.equipped);
            _stateMachine.Player.Animator.SetBool(_stateMachine.Player.AnimationData.EquipTwinToolParameterHash, hand.isTwinTool && quickSlot.itemSlot.equipped);
            _stateMachine.Player.Animator.SetFloat(_stateMachine.Player.AnimationData.BlendEquipDefaultToolParameterHash, hand.isTwoHandedTool && quickSlot.itemSlot.equipped || hand.isTwinTool && quickSlot.itemSlot.equipped ? 0f : 1f);
            _stateMachine.Player.Animator.SetFloat(_stateMachine.Player.AnimationData.BlendEquipTwoHandedToolParameterHash, hand.isTwoHandedTool && quickSlot.itemSlot.equipped ? 1f : 0f);
            _stateMachine.Player.Animator.SetFloat(_stateMachine.Player.AnimationData.BlendEquipTwinToolParameterHash, hand.isTwinTool && quickSlot.itemSlot.equipped ? 1f : 0f);
        }
        else
        {
            _stateMachine.Player.Animator.SetBool(_stateMachine.Player.AnimationData.EquipTwoHandedToolParameterHash, false);
            _stateMachine.Player.Animator.SetBool(_stateMachine.Player.AnimationData.EquipTwinToolParameterHash, false);
            _stateMachine.Player.Animator.SetFloat(_stateMachine.Player.AnimationData.BlendEquipDefaultToolParameterHash, 1f);
            _stateMachine.Player.Animator.SetFloat(_stateMachine.Player.AnimationData.BlendEquipTwoHandedToolParameterHash, 0f);
            _stateMachine.Player.Animator.SetFloat(_stateMachine.Player.AnimationData.BlendEquipTwinToolParameterHash, 0f);
        }
    }
}