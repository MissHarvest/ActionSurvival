using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBuildState : PlayerBaseState
{
    public PlayerBuildState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0;
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        _stateMachine.Player.ToolSystem.OnUnEquip += OnItemEquiped;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        _stateMachine.Player.ToolSystem.OnUnEquip -= OnItemEquiped;
    }

    private void OnItemEquiped(QuickSlot quickSlot)
    {
        // 빌드 상태 나가기
        Debug.Log("Exit Build");
        if(quickSlot.itemSlot.itemData is ToolItemData tooldata)
        {
            if(tooldata.isTwoHandedTool)
            {
                _stateMachine.ChangeState(_stateMachine.TwoHandedToolIdleState);
            }
            else if(tooldata.isTwinTool)
            {
                _stateMachine.ChangeState(_stateMachine.TwinToolIdleState);
            }
            else 
            {
                _stateMachine.ChangeState(_stateMachine.IdleState);
            }
        }        
    }

    protected override void OnInteractStarted(InputAction.CallbackContext context)
    {
        // 건축 상태 시 상호작용 재 입력 시

        //base.OnInteractStarted(context);
    }

    public override void Update()
    {
        
    }

    // _stateMachine.MovementInput 활용!
}
