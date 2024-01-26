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

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        Managers.Game.Player.ToolSystem.OnEquip += OnEquipTypeOfTool;
        Managers.Game.Player.ToolSystem.OnUnEquip += OnUnEquipTypeOfTool;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        Managers.Game.Player.ToolSystem.OnEquip -= OnEquipTypeOfTool;
        Managers.Game.Player.ToolSystem.OnUnEquip -= OnUnEquipTypeOfTool;
    }

    protected virtual void OnMove()
    {
        ToolItemData toolItemDate = (ToolItemData)Managers.Game.Player.ToolSystem.ItemInUse.itemData;

        if (toolItemDate.isTwoHandedTool == true)
        {
            _stateMachine.ChangeState(_stateMachine.TwoHandedToolRunState);
        }
        else if (toolItemDate.isTwinTool == true)
        {
            _stateMachine.ChangeState(_stateMachine.TwinToolRunState);
        }
        else
        {
            _stateMachine.ChangeState(_stateMachine.RunState);
        }
    }

    protected virtual void OnAttack()
    {
        _stateMachine.ChangeState(_stateMachine.ComboAttackState);
    }

    protected virtual void OnEquipTypeOfTool(QuickSlot quickSlot)
    {
        // QuickSlot에 저장된 ItemData 값을 매개 변수로 받아서 TwoHandedTool에 저장
        ItemData equippedItemData = quickSlot.itemSlot.itemData;

        // ItemData에 종속된 class ToolItemData에 bool isTwoHandedTool 변수를 참조하기 위하여 형변환
        ToolItemData equippedToolItemDate = (ToolItemData)equippedItemData;

        // ToolSystem의 event OnEquip에 구독하여 아래의 조건문을 이용하여 애니메이션을 켜고 끈다.

        if (equippedToolItemDate.isWeapon == true && equippedToolItemDate.isTwoHandedTool == true)
        {
            _stateMachine.ChangeState(_stateMachine.TwoHandedToolIdleState);
            Debug.Log("두 손  도구애니메이션 시작");
        }
        else if (equippedToolItemDate.isWeapon == true && equippedToolItemDate.isTwinTool == true)
        {
            _stateMachine.ChangeState(_stateMachine.TwinToolIdleState);
            Debug.Log("한 쌍 도구 애니메이션 시작");
        }
        else
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
    }

    protected virtual void OnUnEquipTypeOfTool(QuickSlot quickSlot)
    {
        _stateMachine.ChangeState(_stateMachine.IdleState);
    }
}
