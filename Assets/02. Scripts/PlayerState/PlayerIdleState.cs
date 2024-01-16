using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(PlayerStateMachine playerStateMachine) :base(playerStateMachine)
    {
        Managers.Game.Player.ToolSystem.OnEquip += OnEquipTwoHandedTool;
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0f;
        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (_stateMachine.MovementInput != Vector2.zero)
        {
            OnMove();
            return;
        }        
    }

    public void OnEquipTwoHandedTool(QuickSlot quickSlot)
    {
        // QuickSlot에 저장된 ItemData 값을 매개 변수로 받아서 TwoHandedTool에 저장
        ItemData TwoHandedTool = quickSlot.itemSlot.itemData;
        // ItemData에 종속된 class ToolItemData에 bool isTwoHandedTool 변수를 참조하기 위하여 형변환
        ToolItemData toolItemDate = (ToolItemData)TwoHandedTool;

        // ToolSystem의 event OnEquip에 구독하여 아래의 조건문을 이용하여 애니메이션을 켜고 끈다.
        if (toolItemDate.isTwoHandedTool == true)
        {
            StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
            StartAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolIdleParameterHash);
            Debug.Log("두 손 애니메이션 시작");
        }
        else
        {
            StopAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolIdleParameterHash);
            StartAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
        }
    }
}
